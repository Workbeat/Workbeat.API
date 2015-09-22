# Workbeat.API Client
Cliente para uso del API de workbeat en .NET

Este es cliente para utilizar las APIs de Workbeat, desarrollado en .NET.

## Uso

Las APIs de workbeat son basadas en REST, utilizando un flujo de OAuth como medio de Autorizacion/Autentificacion. 
Este cliente utiliza exclusivamente el flujo de client_credentials, por lo que solo puede ser utilizado en un ambiente 
de servidor, donde el client_secret esté seguro.

**Este cliente actualmente NO es seguro para utilizarse en aplicaciones las cuales expongan el client_secret de la aplicacion.**
No debe ser usado en aplicaciones de Desktop que son instaladas en computadoras para un usuario final o en las que las llamadas del API son contextuales al usuario. Por su misma naturaleza, este cliente al acceder a las APIs lo hace como administrador y tiene acceso a todas las funciones con todos los permisos.


## Referencia/documentación de APIs
La documentación del API de workbeat está siendo generada. En una versión subsecuente de este documento, estará aquí la liga a la documentacion completa del API.
