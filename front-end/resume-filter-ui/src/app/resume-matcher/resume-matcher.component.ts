import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders, HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MsalService } from '@azure/msal-angular';
import { AuthenticationResult } from '@azure/msal-browser';

@Component({
  selector: 'app-resume-matcher',
  imports: [CommonModule, HttpClientModule, FormsModule],
  templateUrl: './resume-matcher.component.html',
  styleUrl: './resume-matcher.component.scss'
})
export class ResumeMatcherComponent {
resumes: File[] = [];
  jobDescriptionFile?: File;
  results: { name: string; score: number }[] = [];
  loading = false;

  constructor(private http: HttpClient, private msalService: MsalService) {}

  onResumeFilesSelected(event: any) {
    this.resumes = Array.from(event.target.files);
  }

  onJobDescriptionSelected(event: any) {
    this.jobDescriptionFile = event.target.files[0];
  }

login() {
  this.msalService.loginPopup({
    scopes: ['api://e033b5db-c555-4e68-b369-c1eca9f6071a/resume.read'] // Adjust scopes as needed
  }).subscribe({
    next: (loginResponse: AuthenticationResult) => {
      this.msalService.instance.setActiveAccount(loginResponse.account);
    },
    error: (error) => {
      console.error('Login failed:', error);
    }
  });
}
  async onSubmit() {
    if (!this.jobDescriptionFile || this.resumes.length === 0) return;

    const formData = new FormData();
    formData.append('jdFile', this.jobDescriptionFile);
    this.resumes.forEach(file => formData.append('resumes', file));

    this.loading = true;

    try {
      let account = this.msalService.instance.getActiveAccount();
    if (!account) {
      const accounts = this.msalService.instance.getAllAccounts();
      if (accounts.length > 0) {
        account = accounts[0];
        this.msalService.instance.setActiveAccount(account);
      } else {
        throw new Error('No logged in account found. Please login first.');
      }
    }
      const tokenResult: AuthenticationResult = await this.msalService.instance.acquireTokenSilent({
        scopes: ['api://e033b5db-c555-4e68-b369-c1eca9f6071a/resume.read'], // Adjust scope as needed
        account: this.msalService.instance.getAllAccounts()[0]
      });

      const headers = new HttpHeaders({
        Authorization: `Bearer ${tokenResult.accessToken}`
      });

      this.http.post<any[]>('https://localhost:7098/api/Resume/match-resumes', formData, { headers })
        .subscribe({
          next: (data) => {
            this.results = data.sort((a, b) => a.score - b.score); // ascending
            this.loading = false;
          },
          error: (err) => {
            console.error('API Error:', err);
            this.loading = false;
          }
        });
    } catch (err) {
      console.error('Token Error:', err);
      this.loading = false;
    }
  }
}
