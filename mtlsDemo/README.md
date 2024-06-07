# mtlsDemo
- If you are familiar with Visual Studio you can build the projects yourself after examining the code.
- Otherwise, there are 2 zip files in 01-Compiled folder which consists of the Server & Client Application. 
- Both the zip files will contain the executable to demonstrate how mtls works

# Requirements
- Windows Based Machine

# Instructions 

**Certificate Password** : password

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

## PowerShell Scripts Used To Generate the certificates

### Create certs directory
```
New-Item -ItemType Directory -Force -Path "C:\certs"
```
## Server Certificates Creation
### Create Server CA Root Certificate
```
New-SelfSignedCertificate -Type Custom -KeyUsage CertSign, CRLSign, DigitalSignature -KeyAlgorithm RSA -KeyLength 2048 -Subject "CN=ServerRootCA" -CertStoreLocation "Cert:\LocalMachine\My" -KeyExportPolicy Exportable -HashAlgorithm SHA256 -NotAfter (Get-Date).AddYears(10)
```
### Export Server Root CA Certificate
```
$serverRootCACert = Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object { $_.Subject -eq "CN=ServerRootCA" }
Export-Certificate -Cert $serverRootCACert -FilePath "C:\certs\ServerRootCA.cer"
```
### Create Server Certificate signed by Server Root CA
```
New-SelfSignedCertificate -Type Custom -DnsName "server.test.local" -CertStoreLocation "Cert:\LocalMachine\My" -Signer $serverRootCACert -KeyExportPolicy Exportable -HashAlgorithm SHA256 -KeyUsage KeyEncipherment, DigitalSignature -FriendlyName "ServerCert"
```
### Export Server Certificate
```
$serverCert = Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object { $_.FriendlyName -eq "ServerCert" }
Export-PfxCertificate -Cert $serverCert -FilePath "C:\certs\ServerCert.pfx" -Password (ConvertTo-SecureString -String "password" -Force -AsPlainText)
```

## Client Certificates Creation
### Create Client CA Root Certificate
```
New-SelfSignedCertificate -Type Custom -KeyUsage CertSign, CRLSign, DigitalSignature -KeyAlgorithm RSA -KeyLength 2048 -Subject "CN=ClientRootCA" -CertStoreLocation "Cert:\LocalMachine\My" -KeyExportPolicy Exportable -HashAlgorithm SHA256 -NotAfter (Get-Date).AddYears(10)
```
### Export Client Root CA Certificate
```
$clientRootCACert = Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object { $_.Subject -eq "CN=ClientRootCA" }
Export-Certificate -Cert $clientRootCACert -FilePath "C:\certs\ClientRootCA.cer"
```
### Create Client Certificate signed by Client Root CA
```
New-SelfSignedCertificate -Type Custom -DnsName "client.test.local" -CertStoreLocation "Cert:\LocalMachine\My" -Signer $clientRootCACert -KeyExportPolicy Exportable -HashAlgorithm SHA256 -KeyUsage KeyEncipherment, DigitalSignature -FriendlyName "ClientCert"
```
### Export Client Certificate
```
$clientCert = Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object { $_.FriendlyName -eq "ClientCert" }
Export-PfxCertificate -Cert $clientCert -FilePath "C:\certs\ClientCert.pfx" -Password (ConvertTo-SecureString -String "password" -Force -AsPlainText)
```
