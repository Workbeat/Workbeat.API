# Workbeat.API Client
Cliente para uso del API de workbeat en .NET

Este es cliente para utilizar las APIs de Workbeat, desarrollado en .NET.

##Compilación
Esta librería esta generada en Visual Studio 2012 usando .NET 4.0, aunque puede ser compilada sin problemas en Visual Studio 2010.

## Uso

Las APIs de workbeat son basadas en REST, utilizando un flujo de OAuth como medio de Autorizacion/Autentificacion. 
Este cliente utiliza exclusivamente el flujo de client_credentials, por lo que solo puede ser utilizado en un ambiente 
de servidor, donde el client_secret esté seguro.

**Este cliente actualmente NO es seguro para utilizarse en aplicaciones las cuales expongan el client_secret de la aplicacion.**
No debe ser usado en aplicaciones de Desktop que son instaladas en computadoras para un usuario final o en las que las llamadas del API son contextuales al usuario. Por su misma naturaleza, este cliente al acceder a las APIs lo hace como administrador y tiene acceso a todas las funciones con todos los permisos.

### Constructor
#### Parámetros
|Parámetro|Descripcion|
|---|---|
|apiUrl|Opcional. Url donde se encuentra el API de Workbeat. default:https://api.workbeat.com:3000/

```vbnet
  Dim api As New Workbeat.API.Client()
```
... ó tambien... 
```vbnet
  Dim url as String = "https://api.workbeat.com:3000/"
  Dim api As New Workbeat.API.Client(url)
```

### Connect
Para utilizar las APIs de Workbeat, es necesario primero hacer la conexión, para autentificar al cliente.

#### Parámetros
|Parámetro|Descripcion|
|---|---|
|client_id|Identificador de la aplicación externa generada por Workbeat.
|client_secret| Contraseña de la aplicación externa. Esta contraseña es generada por Workbeat.

#### Resultado
Al llamar al método Connect, se regresa un objeto de clase APIResultCode. Este objeto contiene un código, el cual indica si existió un error o se pudo conectar correctamente. En caso de conectarse correctamente, en la propiedad *code* se regresa un código 200. En cualquier otro caso, puede regresar un número, basado en códigos de error de HTTP

```vbnet
		Dim client_id as String = "My Client Id"
		Dim client_secret as String = "My Client Secret"
		Dim api As New Workbeat.API.Client()
		Dim res As Workbeat.API.APIResultCode = api.Connect(clientId, clientSecret)
		
		If res.code=200 Then
		  ' Ok... continuar ...
		  
		Else
		  '... Error de conexion.
		  If res.code = 403 Then
		    ' error de autentificacion 
		  End If
		End If
		
``` 



La clase APIResultCode viene definida de la siguiente manera:
```vbnet
Public Class APIResultCode
	Public code As Integer
	Public message As String
	Public [error] As String
End Class
```

## Métodos
El API, como se dijo anteriormente, está basada en REST, por lo que solo se necesitan los siguientes métodos.

### get(path as String, jsonData as String)
El método **get** es utilizado para traer información acerca del recurso especificado. Éste método no modifica el recurso.
Regresa el resultado en un string formato JSON. 

#### Parámetros
|Parámetro|Descripcion|
|---|---|
|path|Ruta al recurso invocado.
|jsonData| Opcional. String con formato JSON con los parámetros necesarios para la llamada.

Ejemplo:
```vbnet

Dim m_apiUrl As String = "https://api.workbeat.com:3000/"
Dim  clientId As String = "mi-client-id"
Dim clientSecret As String = "mi-client-secret"
' NOTA: Asegurarse que la aplicacion (client_id) tiene permiso de acceder al modulo de adm.
Dim api As New Workbeat.API.Client(m_apiUrl)
Dim res As Workbeat.API.APIResultCode = api.Connect(clientId, clientSecret)
If res.code = 200 Then
  Dim result As String = api.get("adm/empleados", Nothing)
  If result.IndexOf("apellidoPaterno") > 0 Then
     'Procesar resultado
  Else
    ' No se encontro un resultado
  End If
  
  ' Llamada que regresa un listado de empleados, con un filtro como parámetro
  result = api.get("adm/empleados", "{""filtro"":""Armando""}")
  If result.IndexOf("Armando") > 0 Then
     'Procesar resultado
  Else
    ' No se encontro un resultado
  End If
  
End If
```


### post(resourcePath As String, jsonData As String)
Utilizado normalmente para crear un nuevo recurso (empleado, posicion, etc). En algunos casos puede ser utilizado para actualizar un recurso.

#### Parámetros
|Parámetro|Descripcion|
|---|---|
|path|Ruta al recurso invocado.
|jsonData| Opcional. String con formato JSON con los parámetros necesarios para la llamada.

#### Resultado
Normalmente, la creación de un recurso, regresa un string con una representación JSON del mismo. Esta representación incluye el identificador asignado por workbeat al recurso recién creado.


### put(resourcePath As String, jsonData As String) As String
Utilizado para actualizar ciertos tipos de recursos. Dependiendo del recurso, este puede ser actualizado ya sea con put o post.

#### Parámetros
|Parámetro|Descripcion|
|---|---|
|path|Ruta al recurso invocado.
|jsonData| Opcional. String con formato JSON con los parámetros necesarios para la llamada.

#### Resultado
Normalmente, la actualización de un recurso, regresa un string con una representación JSON del mismo.


### delete(resourcePath As String, jsonData As String)
Utilizado para borrar un recurso. Normalmente solo regresa un código de status 200 cuando se pudo hacer el borrado sin problemas.

#### Parámetros
|Parámetro|Descripcion|
|---|---|
|resourcePath|Ruta al recurso invocado.
|jsonData| Opcional. String con formato JSON con los parámetros necesarios para la llamada.


#### Resultado
Como resultado de un borrado, se regresa un código de error. El código 200 significa que el recurso fue borrado correctamente.


## Referencia/documentación de APIs
La documentación del API de workbeat está siendo generada. En una versión subsecuente de este documento, estará aquí la liga a la documentacion completa del API.
