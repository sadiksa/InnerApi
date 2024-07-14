# InnerApi App
Kubectl ile Kubernetes kaynaklarina erisim ve degisim uygulamasidir. Cluster icinde konumlanir. \
Erisim icin [**RBAC**](https://kubernetes.io/docs/reference/access-authn-authz/rbac/) kullanir. (localde KUBECONFIG ile calisir.) \
[https://bootcamp.sadiksa.xyz/innerapi](https://bootcamp.sadiksa.xyz/innerapi) (Prod ortamda swagger kapalidir.)

### Pipeline
Azure Devops Build Pipeline ve Release Pipeline kullanilmistir. \
[Build Pipeline yml](https://dev.azure.com/sadiksahin0511/bootcamp/_git/InnerApi?path=/azure-pipelines.yml&version=GBmain)

### Authentication
[Basic](https://dev.azure.com/sadiksahin0511/bootcamp/_git/InnerApi?path=/InnerApi/BasicAuthenticationHandler.cs&version=GBmain) ile korunmaktadir. \
\
User => **temp_user** \
Password => **temp_password**

### InnerApiClient(UI)
Client uygulamasi icin ilgili repoya gidebilirsiniz. [Client](https://dev.azure.com/sadiksahin0511/bootcamp/_git/InnerApiClient)

### Docker
Client uygulamasi pod icinde kubectl kullandigi icin kubectl [Docker](https://dev.azure.com/sadiksahin0511/bootcamp/_git/InnerApi?path=/InnerApi/Dockerfile&version=GBmain) ile container olusurken /usr/local/bin altina kopyalandi.

### Cloud Bilgileri
AWS sistemlerinde 1 master 2 worker calisan K8s sisteminde calisiyor, \
Ssl sertifikasi icin AWS EC2 LB olusturuldu. Cluster ingress NodePort svc ile tek noktadan cikis verildi.

### Kullanilan Kutuphaneler
K8s tarafindan desteklenen resmi [client kutuphanesi](https://github.com/kubernetes-client/csharp) kullanildi. 