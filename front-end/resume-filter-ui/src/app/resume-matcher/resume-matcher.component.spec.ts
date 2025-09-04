import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResumeMatcherComponent } from './resume-matcher.component';

describe('ResumeMatcherComponent', () => {
  let component: ResumeMatcherComponent;
  let fixture: ComponentFixture<ResumeMatcherComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ResumeMatcherComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ResumeMatcherComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
