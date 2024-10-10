import React from 'react';

export const PrivacyPolicyPage = () => {
  return (
    <div>
      <iframe 
        src="/PrivacyPolicy.html"  // Use a forward slash to indicate the root directory
        title="Privacy Policy" 
        style={{ width: '100%', height: '100vh', border: 'none' }}
      ></iframe>
    </div>
  );
};
