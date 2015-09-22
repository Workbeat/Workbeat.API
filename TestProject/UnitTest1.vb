Imports System.Text
Imports System.Web.Script.Serialization

<TestClass()>
Public Class UnitTest1

    Private testContextInstance As TestContext

	Private m_apiUrl As String = "https://api.workbeat.com:3000/"
	Private clientId As String = "mi-client-id"
	Private clientSecret As String = "mi-client-secret"


    '''<summary>
    '''Gets or sets the test context which provides
    '''information about and functionality for the current test run.
    '''</summary>
    Public Property TestContext() As TestContext
        Get
            Return testContextInstance
        End Get
        Set(ByVal value As TestContext)
            testContextInstance = Value
        End Set
    End Property

#Region "Additional test attributes"
    '
    ' You can use the following additional attributes as you write your tests:
    '
    ' Use ClassInitialize to run code before running the first test in the class
    ' <ClassInitialize()> Public Shared Sub MyClassInitialize(ByVal testContext As TestContext)
    ' End Sub
    '
    ' Use ClassCleanup to run code after all tests in a class have run
    ' <ClassCleanup()> Public Shared Sub MyClassCleanup()
    ' End Sub
    '
    ' Use TestInitialize to run code before running each test
    ' <TestInitialize()> Public Sub MyTestInitialize()
    ' End Sub
    '
    ' Use TestCleanup to run code after each test has run
    ' <TestCleanup()> Public Sub MyTestCleanup()
    ' End Sub
    '
#End Region


	<TestMethod()>
	Public Sub convertJSONStringtoFormEncode()
		Dim api As New Workbeat.API.Client(m_apiUrl)
		Dim jsonData As String = "{""test1"":""valTest1"", ""test2"":""valorTest2""}"
		Dim result As String
		result = Workbeat.API.Client.convertJSONStringtoFormEncode(jsonData)
		Assert.AreEqual(result, "test1=valTest1&test2=valorTest2")

		jsonData = ""
		result = Workbeat.API.Client.convertJSONStringtoFormEncode(jsonData)
		Assert.AreEqual(result, "")

		jsonData = "{""test1"":""valTest1"", ""test2"":""valor Test2""}"
		result = Workbeat.API.Client.convertJSONStringtoFormEncode(jsonData)
		Assert.AreEqual(CStr(result), CStr("test1=valTest1&test2=valor+Test2"))

	End Sub

	<TestMethod()>
	<ExpectedException(GetType(ArgumentException))>
	Public Sub ConexionInvalida()
		Dim api As New Workbeat.API.Client(m_apiUrl)
		Dim res As Workbeat.API.APIResultCode = api.Connect("", "")
	End Sub

	<TestMethod()>
	Public Sub GetEmpleados()
		' NOTA: Asegurarse que la aplicacion (client_id) tiene permiso de acceder al modulo de adm.
		Dim api As New Workbeat.API.Client(m_apiUrl)
		Dim res As Workbeat.API.APIResultCode = api.Connect(clientId, clientSecret)
		Assert.AreEqual(api.access_token, res.message)
		Assert.AreEqual(200, res.code)

		Dim result As String = api.get("adm/empleados", Nothing)
		Assert.IsTrue(result.IndexOf("apellidoPaterno") > 0, "No se encontro un resultado")
		result = api.get("adm/empleados", "{""query"":""Armando""}")
		Assert.IsTrue(result.IndexOf("Armando") > 0, "No se encontro la palabra ""Armando"" dentro del resultado")
	End Sub

	<TestMethod()>
	Public Sub GetEmpleadosConFiltroObject()
		' TODO: Add test logic here
		Dim api As New Workbeat.API.Client(m_apiUrl)
		Dim res As Workbeat.API.APIResultCode = api.Connect(clientId, clientSecret)
		Assert.AreEqual(api.access_token, res.message)
		Assert.AreEqual(200, res.code)

		Dim filtro As New Filtros.FiltroEmpleado
		filtro.filtro = "armando"


		Dim result As String = api.get(Of Filtros.FiltroEmpleado)("adm/empleados", filtro)
		Assert.IsTrue(result.IndexOf("Armando") > 0, "No se encontro un resultado")

		Dim arrEmp As PagedResult(Of Workbeat.API.Entities.Empleado)
		Dim js As New JavaScriptSerializer
		arrEmp = js.Deserialize(Of PagedResult(Of Workbeat.API.Entities.Empleado))(result)

		Assert.IsTrue(arrEmp.totalRows > 0)


	End Sub

	' <TestMethod()>
	'<ExpectedException(GetType(System.Net.WebException))>
	Public Sub VerificarErroresEnConexionTest()
		' error en credenciales
		Dim api As New Workbeat.API.Client(m_apiUrl)
		Dim res As New Workbeat.API.APIResultCode
		Try
			res = api.Connect(clientId, clientSecret & "XXXX")
		Catch ex As System.Net.WebException
			Dim wres As System.Net.HttpWebResponse = ex.Response
			Assert.AreEqual(401, DirectCast(wres.StatusCode, Integer), "Debio regresar un codigo 401 (unauthorized)")
			Throw ex
		End Try

		Try
			res = api.Connect(clientId & "XXX)", clientSecret)
		Catch ex As System.Net.WebException
			Dim wres As System.Net.HttpWebResponse = ex.Response
			Assert.AreEqual(401, DirectCast(wres.StatusCode, Integer), "Debio regresar un codigo 401 (unauthorized)")
			Throw ex
		End Try

	End Sub

	<TestMethod()>
	Public Sub VerificarErroresEnConexion()
		' error en credenciales
		Dim api As New Workbeat.API.Client(m_apiUrl)
		Dim res As New Workbeat.API.APIResultCode
		Dim js As New JavaScriptSerializer
		res = api.Connect(clientId, clientSecret & "XXXX")
		Console.WriteLine(js.Serialize(res))
		Assert.AreEqual(401, res.code, "Debio regresar un codigo 401 (unauthorized)")

		res = api.Connect(clientId & "XXX)", clientSecret)
		Console.WriteLine(js.Serialize(res))
		Assert.AreEqual(401, res.code, "Debio regresar un codigo 401 (unauthorized)")
		
	End Sub

	<TestMethod()>
	Public Sub PostCrearYBorrarOrganizacion()
		Dim api As New Workbeat.API.Client(m_apiUrl)
		Dim res As Workbeat.API.APIResultCode = api.Connect(clientId, clientSecret)
		Assert.AreEqual(api.access_token, res.message)

		Console.WriteLine("Creando organizacion")
		Dim result As String = api.post("org/organizaciones", "{""nombre"":""Organizacion UnitTest""}")
		Console.WriteLine(result)
		Assert.IsTrue(result.IndexOf("Organizacion UnitTest") > 0, "No se creo la organizacion Correctamente")

		Dim js As New JavaScriptSerializer
		Dim org As Workbeat.API.Entities.Organizacion = js.Deserialize(Of Workbeat.API.Entities.Organizacion)(result)
		Console.WriteLine("organizacion " & org.id & "creada")

		Console.WriteLine("Actualizando organizacion " & org.id)
		result = api.post("org/organizaciones/" & org.id, "{""nombre"":""UnitTest otronombre""}")
		org = js.Deserialize(Of Workbeat.API.Entities.Organizacion)(result)
		Console.WriteLine(result)
		Assert.IsTrue(result.IndexOf("UnitTest otronombre") > 0, "No se actualizo la organizacion Correctamente")

		Dim wbe As Workbeat.API.Entities.Organizacion = Workbeat.API.Client.ConvertToWorkbeatEntity(Of Workbeat.API.Entities.Organizacion)(result)

		Console.WriteLine("Borrando organizacion " & org.id)
		Dim statusCode As Integer = api.delete("org/organizaciones/" & org.id, "{""nombre"":""Organizacion UnitTest""}")
		Assert.AreEqual(statusCode, 200)

	End Sub


End Class

