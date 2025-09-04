import { PublicClientApplication } from '@azure/msal-browser';
import { MSAL_INSTANCE, MsalService } from '@azure/msal-angular';
import { EnvironmentProviders, makeEnvironmentProviders } from '@angular/core';

export function msalInstanceFactory() {
  const pca = new PublicClientApplication({
    auth: {
      clientId: 'e033b5db-c555-4e68-b369-c1eca9f6071a',
      authority: 'https://login.microsoftonline.com/e4f872eb-d40f-4481-9015-2bc6201cbadb',
      redirectUri: 'http://localhost:4200'
    }
  });

  return pca.initialize().then(() => pca);
}
