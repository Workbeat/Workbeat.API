Imports System.Configuration
Imports System.Web.Script.Serialization
Imports System.Text
Imports System.Net
Imports System.Web
Imports System.IO

Public Class Client

#Region "Propiedades"

	Private m_APIURL As Uri = New Uri("https://api.workbeat.com:3000/")

	Public ReadOnly Property APIUrl As Uri
		Get
			Return m_APIURL
		End Get
	End Property

	Private m_version As String = "v1"
	Public Property APIVersion() As String
		Get
			Return m_version
		End Get
		Set(ByVal value As String)
			m_version = value
		End Set
	End Property


	Private m_client_id As String
	Public Property client_Id() As String
		Get
			Return m_client_id
		End Get
		Set(ByVal value As String)
			m_client_id = value
		End Set
	End Property

	Private m_client_secret As String
	Public Property client_secret() As String
		Get
			Return m_client_secret
		End Get
		Set(ByVal value As String)
			m_client_secret = value
		End Set
	End Property

	Private m_access_token As String
	Public Property access_token() As String
		Get
			Return m_access_token
		End Get
		Set(ByVal value As String)
			m_access_token = value
		End Set
	End Property


	Private m_expires As DateTime
	Public Property expires() As DateTime
		Get
			Return m_expires
		End Get
		Set(ByVal value As DateTime)
			m_expires = value
		End Set
	End Property



#End Region

#Region "Constructores"

	Public Sub New()
		' Usa el uri default
	End Sub

	Public Sub New(APIUrl As Uri)
		m_APIURL = APIUrl
	End Sub

	Public Sub New(APIUrl As String)
		m_APIURL = New Uri(APIUrl)
	End Sub

#End Region

#Region "CONEXION"

	Public Function Connect(clientId As String, clientSecret As String) As APIResultCode
		If String.IsNullOrEmpty(clientId) OrElse String.IsNullOrEmpty(clientSecret) Then
			Throw New ArgumentException("Los parametros de conexion client_id o client_secret no fueron especificados.")
		End If
		client_Id = clientId
		client_secret = clientSecret

		Dim strBase As String = "client_id={0}&client_secret={1}&grant_type=client_credentials"
		Dim strData As String = String.Format(strBase, clientId, clientSecret)

		Dim wr As HttpWebRequest
		wr = HttpWebRequest.Create(New Uri(m_APIURL.AbsoluteUri & "oauth/token"))
		Dim encoding As UTF8Encoding = New UTF8Encoding()
		Dim data() As Byte = encoding.GetBytes(strData)
		wr.Method = "POST"
		wr.Accept = "application/json; charset=utf-8"
		wr.ContentType = "application/x-www-form-urlencoded"
		wr.ContentLength = data.Length

		Dim postStream As Stream = wr.GetRequestStream()
		postStream.Write(data, 0, data.Length)
		postStream.Close()

		Dim response As HttpWebResponse = Nothing
		Dim jsonResponse As String

		Dim js As New JavaScriptSerializer
		Dim rc As APIResultCode

		Try
			response = CType(wr.GetResponse(), HttpWebResponse)
			Using reader As StreamReader = New StreamReader(response.GetResponseStream())
				jsonResponse = reader.ReadLine()
			End Using
		Catch ex As System.Net.WebException
			Using reader As StreamReader = New StreamReader(ex.Response.GetResponseStream())
				jsonResponse = reader.ReadLine()
			End Using
			Return js.Deserialize(Of APIResultCode)(jsonResponse)
		End Try

		Try
			Dim conResult As APIConnectionResult
			conResult = js.Deserialize(Of APIConnectionResult)(jsonResponse)
			access_token = conResult.access_token
			expires = Date.Now.AddSeconds(conResult.expires_in)
			rc = New APIResultCode
			rc.code = 200
			rc.message = conResult.access_token
		Catch ex As Exception
			rc = js.Deserialize(Of APIResultCode)(jsonResponse)
		End Try
		Return rc
	End Function

#End Region

#Region "API FUNCTIONS"

	' Llamada Get. Los parametros se envian a traves del querystring
	Public Function [get](resourcePath As String, Optional jsonData As String = "") As String
		If Not String.IsNullOrEmpty(resourcePath) Then

			CheckAndConnect()

			Dim wr As HttpWebRequest
			If Not resourcePath.StartsWith("/") Then
				resourcePath = "/" & resourcePath
			End If
			Dim formEncodedData As String
			formEncodedData = "?access_token=" & access_token
			If Not String.IsNullOrEmpty(jsonData) Then
				formEncodedData &= "&" & convertJSONStringtoFormEncode(jsonData)
			End If
			Dim url As String = m_APIURL.AbsoluteUri & APIVersion & resourcePath & formEncodedData
			wr = HttpWebRequest.Create(New Uri(url))
			Dim encoding As UTF8Encoding = New UTF8Encoding()
			wr.Method = "GET"
			wr.Accept = "application/json; charset=utf-8"
			wr.ContentType = "application/json"
			wr.ContentLength = 0


			Dim response As HttpWebResponse = Nothing
			Dim jsonResponse As String
			Try
				response = CType(wr.GetResponse(), HttpWebResponse)
				Using reader As StreamReader = New StreamReader(response.GetResponseStream())
					jsonResponse = reader.ReadLine()
				End Using
			Catch ex As System.Net.WebException
				Using reader As StreamReader = New StreamReader(ex.Response.GetResponseStream())
					jsonResponse = reader.ReadLine()
				End Using
			End Try

			Return jsonResponse
		Else
			Return Nothing
		End If
	End Function

	' Llamada Get Utilizando Objeto de filtro. Internamente deserializa el objeto 
	' y envia los parámetros a traves del querystring
	Public Function [get](Of T)(resourcePath As String, filtro As T)
		If filtro.GetType().IsSubclassOf(GetType(Filtros.Filtro)) Then
			Dim js As New JavaScriptSerializer
			Dim data As String = js.Serialize(filtro)
			Return [get](resourcePath, data)
		Else
			Throw New ArgumentException("Invalid parameter")
		End If

	End Function

	' Las llamadas a post son siempre con datos de tipo JSON incluidos en el body.
	' No hay datos en el querystring.
	Public Function post(resourcePath As String, jsonData As String) As String
		If Not String.IsNullOrEmpty(resourcePath) Then

			CheckAndConnect()

			Dim wr As HttpWebRequest
			If Not resourcePath.StartsWith("/") Then
				resourcePath = "/" & resourcePath
			End If
			Dim formEncodedData As String
			formEncodedData = "?access_token=" & access_token
			Dim url As String = m_APIURL.AbsoluteUri & APIVersion & resourcePath & formEncodedData
			wr = HttpWebRequest.Create(New Uri(url))
			Dim encoding As UTF8Encoding = New UTF8Encoding()
			Dim data() As Byte = encoding.GetBytes(jsonData)
			wr.Method = "POST"
			wr.Accept = "application/json; charset=utf-8"
			wr.ContentType = "application/json"
			wr.ContentLength = data.Length


			Dim postStream As Stream = wr.GetRequestStream()
			postStream.Write(data, 0, data.Length)
			postStream.Close()

			Dim response As HttpWebResponse = Nothing
			Dim jsonResponse As String
			Try
				response = CType(wr.GetResponse(), HttpWebResponse)
				Using reader As StreamReader = New StreamReader(response.GetResponseStream())
					jsonResponse = reader.ReadLine()
				End Using
			Catch ex As System.Net.WebException
				Using reader As StreamReader = New StreamReader(ex.Response.GetResponseStream())
					jsonResponse = reader.ReadLine()
				End Using
			End Try

			Return jsonResponse
		Else
			Return Nothing
		End If
	End Function

	' Las llamadas a post son siempre con datos de tipo JSON incluidos en el body.
	' No hay datos en el querystring.
	Public Function put(resourcePath As String, jsonData As String) As String
		If Not String.IsNullOrEmpty(resourcePath) Then

			CheckAndConnect()

			Dim wr As HttpWebRequest
			If Not resourcePath.StartsWith("/") Then
				resourcePath = "/" & resourcePath
			End If
			Dim formEncodedData As String
			formEncodedData = "?access_token=" & access_token
			Dim url As String = m_APIURL.AbsoluteUri & APIVersion & resourcePath & formEncodedData
			wr = HttpWebRequest.Create(New Uri(url))
			Dim encoding As UTF8Encoding = New UTF8Encoding()
			Dim data() As Byte = encoding.GetBytes(jsonData)
			wr.Method = "PUT"
			wr.Accept = "application/json; charset=utf-8"
			wr.ContentType = "application/json"
			wr.ContentLength = data.Length
			Dim postStream As Stream = wr.GetRequestStream()
			postStream.Write(data, 0, data.Length)
			postStream.Close()

			Dim response As HttpWebResponse = Nothing
			Dim jsonResponse As String
			Try
				response = CType(wr.GetResponse(), HttpWebResponse)
				Using reader As StreamReader = New StreamReader(response.GetResponseStream())
					jsonResponse = reader.ReadLine()
				End Using
			Catch ex As System.Net.WebException
				Using reader As StreamReader = New StreamReader(ex.Response.GetResponseStream())
					jsonResponse = reader.ReadLine()
				End Using
			End Try

			Return jsonResponse
		Else
			Return Nothing
		End If
	End Function

	' Las llamadas a delete son opcionalmente con datos de tipo JSON incluidos en el body.
	' No hay datos en el querystring, pero el id del recurso a borrar se incluye ahi.
	Public Function delete(resourcePath As String, Optional jsonData As String = "") As Integer
		If Not String.IsNullOrEmpty(resourcePath) Then
			CheckAndConnect()

			Dim wr As HttpWebRequest
			If Not resourcePath.StartsWith("/") Then
				resourcePath = "/" & resourcePath
			End If
			Dim formEncodedData As String
			formEncodedData = "?access_token=" & access_token
			Dim url As String = m_APIURL.AbsoluteUri & APIVersion & resourcePath & formEncodedData
			wr = HttpWebRequest.Create(New Uri(url))
			Dim encoding As UTF8Encoding = New UTF8Encoding()
			Dim data() As Byte = encoding.GetBytes(jsonData)
			wr.Method = "DELETE"
			wr.Accept = "application/json; charset=utf-8"
			wr.ContentType = "application/json"
			wr.ContentLength = data.Length

			If data.Length > 0 Then
				Dim postStream As Stream = wr.GetRequestStream()
				postStream.Write(data, 0, data.Length)
				postStream.Close()
			End If

			Dim response As HttpWebResponse = Nothing
			response = CType(wr.GetResponse(), HttpWebResponse)

			Return response.StatusCode

		Else
			Return 400 ' bad request
		End If
	End Function



#End Region

#Region "Utilerias Publicas"

#Region "Object Conversion"

	Public Shared Function convertJSONStringtoFormEncode(jsonString As String) As String
		If String.IsNullOrEmpty(jsonString) Then
			Return ""
		Else
			Dim formEncodedData As String = String.Empty
			Dim dict As New Dictionary(Of String, String)
			Dim js As New JavaScriptSerializer
			Dim kvp As KeyValuePair(Of String, String)
			dict = js.Deserialize(Of Dictionary(Of String, String))(jsonString)
			For Each kvp In dict
				If formEncodedData.Length > 0 Then
					formEncodedData &= "&"
				End If
				formEncodedData &= HttpUtility.UrlEncode(kvp.Key) & "=" & HttpUtility.UrlEncode(kvp.Value)
			Next
			Return formEncodedData
		End If
	End Function

	Public Shared Function convertToErrorCodeObject(result As String) As APIErrorCode
		Dim js As New JavaScriptSerializer
		Dim errCode As APIErrorCode
		Try
			errCode = js.Deserialize(Of APIErrorCode)(result)
		Catch ex As Exception
			errCode = Nothing
		End Try
		Return errCode
	End Function

	Public Shared Function convertToResultCodeObject(result As String) As APIResultCode
		Dim js As New JavaScriptSerializer
		Dim resCode As APIResultCode
		Try
			resCode = js.Deserialize(Of APIResultCode)(result)
		Catch ex As Exception
			resCode = Nothing
		End Try
		Return resCode
	End Function

	Public Shared Function ConvertToWorkbeatEntity(Of T)(jsonString As String) As T
		Dim js As New JavaScriptSerializer
		Dim wbe As T
		wbe = js.Deserialize(Of T)(jsonString)
		Return wbe
	End Function

#End Region

#Region "Access Token Setting"

	Public Sub setAccessToken(accessToken As String, expireSeconds As Integer)
		access_token = accessToken
		expires = Now.AddSeconds(expireSeconds)
	End Sub

	Public Sub setAccessToken(accessToken As String, expireDate As DateTime)
		access_token = accessToken
		expires = expireDate
	End Sub

#End Region

#End Region

#Region "Utilerias Privadas"

	Private Sub CheckAndConnect()
		If expires < Now Then
			Connect(client_Id, client_secret)
		End If
	End Sub

#End Region

End Class


Public Class APIResultCode
	Public code As Integer
	Public message As String
	Public [error] As String
	' Public error_description As String
End Class

Public Class APIErrorCode
	Public [error] As APIResultCode
End Class


Friend Class APIConnectionResult
	Public token_type As String
	Public access_token As String
	Public expires_in As Integer
End Class

Public Class PagedResult(Of T)
	Public page As Integer
	Public totalRows As Integer
	Public pagesize As Integer
	Public pagestart As Integer
	Public data() As T
End Class

