Namespace Entities
	Public Class WorkbeatEntity

	End Class

	Public Class Organizacion
		Inherits WorkbeatEntity
		Public id As Integer
		Public nombre As String
		Public fechaCreacion As DateTime
		Public fechaUltimoCambio As DateTime
	End Class

	Public Class Posicion
		Inherits WorkbeatEntity
		Public id As Integer
		Public nombre As String
		Public nombreOrganizacion As String
		Public codigo As String
		Public fechaCreacion As DateTime
		Public fechaUltimoCambio As DateTime
		Public atributoDefault As atributoPosicion
	End Class

	Public Class Empleado
		Inherits WorkbeatEntity
		Public id As Integer
		Public nombre As String
		Public apellidoPaterno As String
		Public apellidoMaterno As String
		Public posiciones() As Posicion
	End Class

	Public Class atributoPosicion
		Inherits WorkbeatEntity
		Public nombreAtributo As String
		Public referencia As String
		Public nombre As String
	End Class

End Namespace


Namespace Filtros

	Public Class Filtro
		Public filtro As String = String.Empty
		Public pagina As Integer = 1
		Public tamanoPagina As Integer = 100
		Public orden As String = String.Empty
	End Class

	Public Class FiltroEmpleado
		Inherits Filtro
		Public incluirBajas As String = String.Empty
	End Class

	Public Class FiltroPosicion
		Inherits Filtro
		Public idPosicionReporta As Integer
	End Class

	Public Class FiltroPais
		Inherits Filtro
		Public estado As String = String.Empty
		Public pais As String = String.Empty
	End Class


	Public Class FiltroMovPersonal
		Inherits Filtro
		Public tipoMovimiento As String = String.Empty
		Public fechaInicial As String = String.Empty
		Public fechaFinal As String = String.Empty
	End Class


End Namespace
