# Workbeat.API Client
Cliente para uso del API de workbeat en .NET

Este es cliente para utilizar las APIs de Workbeat, desarrollado en .NET.

## Uso

Las APIs de workbeat son basadas en REST, utilizando un flujo de OAuth como medio de Autorizacion/Autentificacion. 
Este cliente utiliza exclusivamente el flujo de client_credentials, por lo que solo puede ser utilizado en un ambiente 
de servidor, donde el client_secret esté seguro. 

**Este cliente NO debe utilizarse en aplicaciones las cuales expongan el client_secret de la aplicacion**


## Referencia de APIs
La documentación del API de workbeat
