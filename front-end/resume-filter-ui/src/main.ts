import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { provideHttpClient } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';
import { MSAL_INSTANCE, MsalService } from '@azure/msal-angular';
import { PublicClientApplication } from '@azure/msal-browser';

const msalInstance = new PublicClientApplication({
  auth: {
    clientId: 'e033b5db-c555-4e68-b369-c1eca9f6071a',
    authority: 'https://login.microsoftonline.com/e4f872eb-d40f-4481-9015-2bc6201cbadb',
    redirectUri: 'http://localhost:4200'
  }
});

async function bootstrap() {
  await msalInstance.initialize();

  bootstrapApplication(AppComponent, {
    providers: [
      provideHttpClient(),
      provideRouter(routes),
      provideAnimations(),
      {
        provide: MSAL_INSTANCE,
        useValue: msalInstance
      },
      MsalService
    ]
  });
}

bootstrap();
