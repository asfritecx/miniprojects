# mtlsDemo
- If you are familiar with Visual Studio you can build the projects yourself after examining the code.
- Otherwise, there are 2 zip files in 01-Compiled folder which consists of the Server & Client Application. 
- Both the zip files will contain the executable to demonstrate how mtls works

# Requirements
- Windows Based Machine

# Instructions 
- Edit your host file to point to the server change the ip address if you are trying on separate machines

'c:\windows\system32\drivers\etc\hosts'
```
127.0.0.1 server.test.local
```

- Unzip Both ServerApp and ClientApp
- Open the certs folder in ServerApp
- Double click on the ServerRootCA.cer Select **Local Machine** & install the cert into the **Trusted Root Authority**
- Double click on the ClientRootCA.cer Select **Local Machine** & install the cert into the **Trusted Root Authority**
- Double Click on the ServerApp Executable
- Try to visit the url at server.test.local via chrome/edge/chromium based browser (It should fail)
- Open Powershell in the root of ClientApp Directory and launch the executable in powershell with ./clientapp.exe
- The request should return ["Hello","World"]

# Cleanup
- Search For Computer Certificates and remove the ServerRootCA & ClientRootCA Certs

