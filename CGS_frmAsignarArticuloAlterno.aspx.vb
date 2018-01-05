'-------------------------------------------------------------------------------------------'
' Inicio del codigo
'-------------------------------------------------------------------------------------------'
Imports vis3Controles.wbcAdministradorMensajeModal
Imports Microsoft.VisualBasic
Imports cusAplicacion
Imports cusDatos
Imports System.Data
Imports vis1Controles

'-------------------------------------------------------------------------------------------'
' Inicio de clase "frmPlantilla"
'-------------------------------------------------------------------------------------------'
Partial Class CGS_frmAsignarArticuloAlterno
    Inherits vis2formularios.frmFormularioGenerico

#Region "Declaraciones"

#End Region

#Region "Propiedades"

    Private Property pcOrigenDocumento() As String
        Get
            Return CStr(Me.ViewState("pcOrigenDocumento"))
        End Get
        Set(ByVal value As String)
            Me.ViewState("pcOrigenDocumento") = value
        End Set
    End Property

    Private Property pcOrigenRenglon() As String
        Get
            Return CStr(Me.ViewState("pcOrigenRenglon"))
        End Get
        Set(ByVal value As String)
            Me.ViewState("pcOrigenRenglon") = value
        End Set
    End Property

#End Region

#Region "Eventos"

    Protected Sub mCargaPagina(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'La primera vez que se cargue el formulario...
        If Not Me.IsPostBack() Then


            Me.txtCod_Art.mConfigurarBusqueda("Articulos", _
                                              "Cod_Art,Nom_Art", _
                                              "Cod_Art,Nom_Art,Status", _
                                              ".,Código,Nombre,Estatus", _
                                              "Cod_Art,Nom_Art", _
                                              "../../Framework/Formularios/frmFormularioBusqueda.aspx", _
                                              "Cod_Art,Nom_Art", _
                                              "", "")

            Dim laParametros As Generic.Dictionary(Of String, Object)

            laParametros = Me.Session("frmComplementos.paParametros")
            Me.Session.Remove("frmComplementos.paParametros")

            If laParametros IsNot Nothing Then

                'Lee la colección de campos índice del formulario de origen
                Dim laIndices As Generic.Dictionary(Of String, Object)
                laIndices = laParametros("laIndices")

                Me.pcOrigenDocumento = CStr(laIndices("Documento")).Trim()
                Me.pcOrigenRenglon = CStr(laIndices("Renglon")).Trim()

            End If

            Dim loConsulta As New StringBuilder()

            loConsulta.AppendLine("SELECT   Renglones_OCompras.Cod_Art      AS Cod_Art,")
            loConsulta.AppendLine("         Articulos.Nom_Art               AS Nom_Art,")
            loConsulta.AppendLine("         Renglones_OCompras.Caracter1    AS Art_Alt")
            loConsulta.AppendLine("FROM Renglones_OCompras")
            loConsulta.AppendLine(" JOIN Articulos ON Articulos.Cod_Art = Renglones_OCompras.Cod_Art")
            loConsulta.AppendLine("WHERE Renglones_OCompras.Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento))
            loConsulta.AppendLine(" AND Renglones_OCompras.Renglon = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon))
            loConsulta.AppendLine("")
            loConsulta.AppendLine("")


            Dim loRenglones As DataTable = (New goDatos()).mObtenerTodosSinEsquema(loConsulta.ToString(), "Renglones_OCompras").Tables(0)
            'VERIFICA QUE EL RENGLÓN SELECCIONADO NO TENGA YA UN ARTICULO ALTERNO
            If CStr(loRenglones.Rows(0).Item("Art_Alt")).Trim() <> "" Then
                Me.mMostrarMensajeModal("Operación no permitida", "Este renglón ya tiene un artículo alterno.", "a")
                Me.cmdAceptar.Enabled = False
                Return
            End If

            Me.lblArticulo.Text = CStr(loRenglones.Rows(0).Item("Cod_Art")).Trim() & ":  " & CStr(loRenglones.Rows(0).Item("Nom_Art")).Trim()
            Me.lblRenglon.Text = " " & Me.pcOrigenRenglon

        End If

    End Sub

    Protected Sub cmdAceptar_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles cmdAceptar.Click

        If CStr(Me.txtCod_Art.pcTexto("Cod_Art")).Trim() <> "" Then

            Dim loConsulta As New StringBuilder()

            loConsulta.AppendLine("")
            loConsulta.AppendLine("UPDATE Renglones_OCompras")
            loConsulta.AppendLine("SET Caracter1 = " & goServicios.mObtenerCampoFormatoSQL(CStr(Me.txtCod_Art.pcTexto("Cod_Art"))).Trim())
            loConsulta.AppendLine("WHERE Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento))
            loConsulta.AppendLine(" AND Renglon = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon))
            loConsulta.AppendLine("")

            Dim lodatos As New goDatos()

            Try
                Dim laSentencias As New ArrayList()
                laSentencias.Add(loConsulta.ToString())
                'EJECUTAR EL UPDATE, EL ARTÍCULO ALTERNO SE GUARDA EN EL CAMPO caracter1
                lodatos.mEjecutarTransaccion(laSentencias)

                Me.mMostrarMensajeModal("Artículo asignado", "Se asignó el artículo " & Me.txtCod_Art.pcTexto("Cod_Art") & " - " & Me.txtCod_Art.pcTexto("Nom_Art") & " como alterno. ", "i", False)
                Me.cmdAceptar.Enabled = False

            Catch ex As Exception

                Me.mMostrarMensajeModal("Proceso no completado", "No fue posible asignar el artículo alterno.", "e", True)

            End Try
        Else
            Me.mMostrarMensajeModal("Proceso no completado", "No ha seleccionado ningún artículo.", "i", True)

        End If
    End Sub



#End Region

#Region "Metodos"

    ''' <summary>
    ''' Muestra un mensaje modal en pantalla.
    ''' </summary>
    ''' <param name="lcTitulo"></param>
    ''' <param name="lcContenido"></param>
    ''' <param name="lcTipo"></param>
    ''' <remarks></remarks>
    Private Sub mMostrarMensajeModal(lcTitulo As String, lcContenido As String, lcTipo As String, Optional llMensajeLargo As Boolean = False)
        Dim loScript As New StringBuilder()

        lcTipo = lcTipo.ToLower()

        If (lcTipo <> "a") AndAlso _
            (lcTipo <> "e") AndAlso _
            (lcTipo <> "i") Then

            lcTipo = "a"

        End If

        loScript.Append("(function(){")
        loScript.Append("    var w = window.innerWidth - 20;")
        loScript.Append("    var h = window.innerHeight - 20;")
        If llMensajeLargo Then
            loScript.Append("    w = Math.max(0, 600 - w);")
            loScript.Append("    h = Math.max(0, 500 - h);")
        Else
            loScript.Append("    w = Math.max(0, 500 - w);")
            loScript.Append("    h = Math.max(0, 250 - h);")
        End If
        loScript.Append("    window.resizeBy(w,h);")
        loScript.Append("})();")
        loScript.Append("")

        loScript.Append("window.poMensajes.mMostrarMensajeModal('")
        loScript.Append(lcTitulo.Replace("'", "\'").Replace(vbNewLine, " "))
        loScript.Append("','")
        loScript.Append(lcContenido.Replace("'", "\'").Replace(vbNewLine, "\n"))
        loScript.Append("','")
        loScript.Append(lcTipo)
        If llMensajeLargo Then
            loScript.AppendLine("', 500, 600);")
        Else
            loScript.AppendLine("', 250, 500);")
        End If

        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Mensaje", loScript.ToString(), True)

    End Sub

    ''' <summary>
    ''' Muestra un mensaje modal en pantalla.
    ''' </summary>
    ''' <param name="lcTitulo"></param>
    ''' <param name="lcContenido"></param>
    ''' <param name="lcTipo"></param>
    ''' <remarks></remarks>
    Private Sub mMostrarMensajeNoModal(lcTitulo As String, lcContenido As String, lcTipo As String, Optional llMensajeLargo As Boolean = False)
        Dim loScript As New StringBuilder()

        lcTipo = lcTipo.ToLower()

        If (lcTipo <> "a") AndAlso _
            (lcTipo <> "e") AndAlso _
            (lcTipo <> "i") Then

            lcTipo = "a"

        End If

        loScript.Append("window.poMensajes.mMostrarMensajeNoModal('")
        loScript.Append(lcTitulo.Replace("'", "\'").Replace(vbNewLine, " "))
        loScript.Append("','")
        loScript.Append(lcContenido.Replace("'", "\'").Replace(vbNewLine, "\n"))
        loScript.Append("','")
        loScript.Append(lcTipo)
        loScript.AppendLine("', 100, 500);")

        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Mensaje", loScript.ToString(), True)

    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        ScriptManager.RegisterStartupScript(Me, Me.GetType, "EnlazarControles", ";loGenerador.mEnlazarControles();", True)

    End Sub

    'SI LA BÚSQUEDA ES VÁLIDA SE MUESTRA UNA ADVERTENCIA DEL ARTÍCULO QUE SE VA A ASIGNAR
    Protected Sub txtCod_Art_mResultadoBusquedaValido(ByVal sender As vis1Controles.txtNormal, lcNombreCampo As String, lnIndice As Integer) Handles txtCod_Art.mResultadoBusquedaValido
        Me.lblAdvertencia.Text = "Se asignará el artículo " & Me.txtCod_Art.pcTexto("Cod_Art") & " - " & Me.txtCod_Art.pcTexto("Nom_Art") & " como alterno. "

    End Sub

    Protected Sub txtCod_Art_mResultadoBusquedaNoValido(ByVal sender As vis1Controles.txtNormal, lcNombreCampo As String, lnIndice As Integer) Handles txtCod_Art.mResultadoBusquedaNoValido
        Me.mMostrarMensajeModal("Advertencia", _
            "El artículo indicado no es válido.", "a")

        Me.txtCod_Art.pcTexto("Cod_Art") = ""
    End Sub

#End Region



End Class
'-------------------------------------------------------------------------------------------'
' Fin del codigo																			'
'-------------------------------------------------------------------------------------------'
' KDE: 01/07/16: Codigo Inicial.								                            '
'-------------------------------------------------------------------------------------------'
' 
