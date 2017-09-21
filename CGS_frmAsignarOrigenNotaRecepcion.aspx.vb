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
Partial Class CGS_frmAsignarOrigenNotaRecepcion
    Inherits vis2formularios.frmFormularioGenerico

#Region "Declaraciones"
    Private Const KN_CANTIDAD_RENGLONES_LOTE As Integer = 0
#End Region

#Region "Propiedades"

    'Private Property plSoloLectura As Boolean
    '    Get
    '        Return CBool(Me.ViewState("plSoloLectura"))
    '    End Get
    '    Set(value As Boolean)
    '        Me.ViewState("plSoloLectura") = value
    '    End Set
    'End Property

    Private Property pnDecimalesParaCantidad As Integer
        Get
            Return CInt(Me.ViewState("pnDecimalesParaCantidad"))
        End Get
        Set(value As Integer)
            Me.ViewState("pnDecimalesParaCantidad") = value
        End Set
    End Property
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
    Private Property pcRenglonSelected() As String
        Get
            Return CStr(Me.ViewState("pcRenglonSelected"))
        End Get
        Set(ByVal value As String)
            Me.ViewState("pcRenglonSelected") = value
        End Set
    End Property

#End Region

#Region "Eventos"

    Protected Sub mCargaPagina(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'La primera vez que se cargue el formulario...
        If Not Me.IsPostBack() Then

            Me.TxtBusqueda.mConfigurarBusqueda("Ordenes_Compras", _
                                              "Documento", _
                                              "Documento,Comentario,status", _
                                              ".,Documento,Comentario,Estatus", _
                                              "Documento,Comentario,status", _
                                              "../../Framework/Formularios/frmFormularioBusqueda.aspx", _
                                              "Documento", _
                                              "", "Status IN ('Procesado','Afectado','Confirmado')")

            'Me.mInicializar()
            Me.pnDecimalesParaCantidad = goOpciones.pnDecimalesParaCantidad

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
        End If

        Me.grdRenglones.mRegistrarColumna("cod_art", "Código", "", True, False, "String", False, 100)
        Me.grdRenglones.mRegistrarColumna("nom_art", "Artículo", "", True, False, "String", False, 300)
        Me.grdRenglones.mRegistrarColumna("can_art", "Cantidad", 0D, True, False, "Decimal", False, 100)
        Me.grdRenglones.mRegistrarColumna("art_alt", "Alterno", "", True, False, "String", False, 100)

        Me.grdRenglones.mLimitarCampoTexto("cod_art", True, 50)
        Me.grdRenglones.mLimitarCampoTexto("nom_art", True, 50)
        Me.grdRenglones.pnDecimalesColumna("can_art") = Me.pnDecimalesParaCantidad
        Me.grdRenglones.mLimitarCampoTexto("art_alt", True, 50)

        If Not Me.IsPostBack() Then

            Dim loConsulta As New StringBuilder()

            loConsulta.AppendLine("SELECT   Renglones_Recepciones.Cod_Art   AS Cod_Art,")
            loConsulta.AppendLine("         Articulos.Nom_Art               AS Nom_Art,")
            loConsulta.AppendLine("         Renglones_Recepciones.Cod_Alm   AS Cod_Alm,")
            loConsulta.AppendLine("         Almacenes.Nom_Alm               AS Nom_Alm,")
            loConsulta.AppendLine("         Renglones_Recepciones.Can_Art1  AS Can_Art,")
            loConsulta.AppendLine("         Renglones_Recepciones.Doc_Ori   AS Doc_Ori")
            loConsulta.AppendLine("FROM Renglones_Recepciones")
            loConsulta.AppendLine(" JOIN Articulos ON Articulos.Cod_Art = Renglones_Recepciones.Cod_Art")
            loConsulta.AppendLine(" JOIN Almacenes ON Almacenes.Cod_Alm = Renglones_Recepciones.Cod_Alm")
            loConsulta.AppendLine("WHERE Renglones_Recepciones.Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento))
            loConsulta.AppendLine(" AND Renglones_Recepciones.Renglon = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon))
            loConsulta.AppendLine("")
            loConsulta.AppendLine("")


            Dim loRenglones As DataTable = (New goDatos()).mObtenerTodosSinEsquema(loConsulta.ToString(), "Renglones_Recepciones").Tables(0)

            If CStr(loRenglones.Rows(0).Item("Doc_Ori")).Trim() <> "" Then
                Me.mMostrarMensajeModal("Operación no permitida", "Este renglón ya tiene un origen asociado.", "a")
                Me.cmdAceptar.Enabled = False
                Return
            End If

            Me.lblArticulo.Text = CStr(loRenglones.Rows(0).Item("Cod_Art")).Trim() & ":  " & CStr(loRenglones.Rows(0).Item("Nom_Art")).Trim()
            Me.lblAlmacen.Text = CStr(loRenglones.Rows(0).Item("Cod_Alm")).Trim() & ":  " & CStr(loRenglones.Rows(0).Item("Nom_Alm")).Trim()
            Me.lblRenglon.Text = " " & Me.pcOrigenRenglon
            Me.lblCantidad.Text = goServicios.mObtenerFormatoCadena(CDec(loRenglones.Rows(0).Item("Can_Art")), goServicios.enuOpcionesRedondeo.KN_RedondeoPuntoMedio, Me.pnDecimalesParaCantidad)

            Me.mCargarTablaVacia()

        Else

            Me.grdRenglones.DataBind()

        End If

        Me.grdRenglones.mHabilitarBotonera(True)
        Me.grdRenglones.plPermitirAgregarRenglon = False
        Me.grdRenglones.plPermitirEliminarRenglon = False
        Me.grdRenglones.mAlmacenarRenglones()

    End Sub

    Protected Sub cmdAceptar_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles cmdAceptar.Click

        If Me.pcRenglonSelected <> 0 Then
            Dim lcConsultaOrden As String = "SELECT Precio1,Caracter1 FROM Renglones_OCompras WHERE Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.TxtBusqueda.pcTexto("Documento")) & "AND Renglon = " & goServicios.mObtenerCampoFormatoSQL(Me.pcRenglonSelected)

            Dim loConsultaOrden As DataTable = (New goDatos()).mObtenerTodosSinEsquema(lcConsultaOrden, "Renglones_OCompras").Tables(0)

            Dim loConsulta As New StringBuilder()

            loConsulta.AppendLine("")
            loConsulta.AppendLine("UPDATE Renglones_Recepciones")
            loConsulta.AppendLine("SET Doc_Ori = " & goServicios.mObtenerCampoFormatoSQL(Me.TxtBusqueda.pcTexto("Documento")) & ",")
            If CStr(loConsultaOrden.Rows(0).Item("Caracter1")).Trim() = "" Then
                loConsulta.AppendLine(" Ren_Ori = " & goServicios.mObtenerCampoFormatoSQL(Me.pcRenglonSelected) & ",")
            End If
            loConsulta.AppendLine(" Precio1 = " & goServicios.mObtenerCampoFormatoSQL(CDec(loConsultaOrden.Rows(0).Item("Precio1"))))
            loConsulta.AppendLine("WHERE Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento))
            loConsulta.AppendLine(" AND Renglon = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon))
            loConsulta.AppendLine("")

            Dim lodatos As New goDatos()

            Try
                Dim laSentencias As New ArrayList()
                laSentencias.Add(loConsulta.ToString())

                lodatos.mEjecutarTransaccion(laSentencias)
                Me.mCargarTablaVacia()

                Me.mMostrarMensajeModal("Origen asignado", "Se asignó la orden de compra " & Me.TxtBusqueda.pcTexto("Documento") & ", renglón " & Me.pcRenglonSelected & " como origen. ", "i", False)

            Catch ex As Exception

                Me.mMostrarMensajeModal("Proceso no completado", "No fue posible asignar el origen.", "e", True)

            End Try
        Else
            Me.mMostrarMensajeModal("Proceso no completado", "No ha seleccionado ningún renglón.", "i", True)

        End If
    End Sub



#End Region

#Region "Metodos"

    Protected Sub mValidar()
        Dim lcConsultaOrden As String = "SELECT Cod_Art,Can_Pen1, Caracter1 FROM Renglones_OCompras WHERE Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.TxtBusqueda.pcTexto("Documento")) & "AND Renglon = " & goServicios.mObtenerCampoFormatoSQL(Me.pcRenglonSelected)

        Dim loConsultaOrden As DataTable = (New goDatos()).mObtenerTodosSinEsquema(lcConsultaOrden, "Renglones_OCompras").Tables(0)

        Dim lcArticulo As String = Me.lblArticulo.Text.Substring(0, 8)

        If (CDec(loConsultaOrden.Rows(0).Item("Can_Pen1")) > 0 And CStr(loConsultaOrden.Rows(0).Item("Cod_Art")).Trim() = lcArticulo) Then
            Me.mMostrarMensajeModal("Operación no permitida", "El renglón seleccionado todavía tiene pendiente, utilizar opción Anexar Documentos.", "e", True)
            Me.pcRenglonSelected = 0
        ElseIf (CStr(loConsultaOrden.Rows(0).Item("Caracter1")).Trim() <> CStr(lcArticulo).Trim() And CStr(loConsultaOrden.Rows(0).Item("Cod_Art")).Trim() <> CStr(lcArticulo).Trim()) Then
            Me.mMostrarMensajeModal("Operación no permitida", "El artículo recibido no coincide con los artículos de la orden de compra.", "e", True)
            Me.pcRenglonSelected = 0
        End If

    End Sub

    ''' <summary>
    ''' Carga la tabla inicial en blanco para el grid de artículos.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub mCargarTablaVacia()


        Dim loTabla As New DataTable("Articulos")

        loTabla.Columns.Add(New DataColumn("renglon", GetType(Integer)))
        loTabla.Columns.Add(New DataColumn("cod_art", GetType(String)))
        loTabla.Columns.Add(New DataColumn("nom_art", GetType(String)))
        loTabla.Columns.Add(New DataColumn("can_art", GetType(Decimal)))

        For i As Integer = 1 To KN_CANTIDAD_RENGLONES_LOTE 'CDec(loRenglones.Rows(0).Item("Total"))
            Dim loRenglon As DataRow = loTabla.NewRow()

            loRenglon("Renglon") = i
            loRenglon("cod_art") = ""
            loRenglon("nom_art") = ""
            loRenglon("can_art") = 0D

            loTabla.Rows.Add(loRenglon)
        Next

        Me.grdRenglones.poOrigenDeDatos = loTabla

        Me.grdRenglones.DataBind()
        Me.grdRenglones.mAlmacenarRenglones()

    End Sub

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

    Private Sub TxtBusqueda_mResultadoBusquedaValido(sender As txtNormal, lcNombreCampo As String, lnIndice As Integer) Handles TxtBusqueda.mResultadoBusquedaValido
        'Coloca comentario
        Dim lcDocumento As String = goServicios.mObtenerCampoFormatoSQL(Me.TxtBusqueda.pcTexto("Documento"))

        Dim lcConsultaOrden As String = "SELECT Comentario FROM Ordenes_Compras WHERE Documento = " & lcDocumento

        Dim loTConsulta As DataTable = (New goDatos()).mObtenerTodosSinEsquema(lcConsultaOrden, "Articulos").Tables(0)

        Me.TxtComentario.Text = CStr(loTConsulta.Rows(0).Item("Comentario")).Trim()

        'Llena renglones
        Dim lcConsulta As New StringBuilder()

        lcConsulta.AppendLine("SELECT Renglones_OCompras.Renglon    AS Renglon,")
        lcConsulta.AppendLine("       Renglones_OCompras.Cod_Art    AS Cod_Art,")
        lcConsulta.AppendLine("       Articulos.Nom_Art             AS Nom_Art,")
        lcConsulta.AppendLine("       Renglones_OCompras.Can_Art1   AS Can_Art,")
        lcConsulta.AppendLine("       Renglones_OCompras.Caracter1  AS Art_Alt,")
        lcConsulta.AppendLine("       Total.numRows					AS Total")
        lcConsulta.AppendLine("FROM Renglones_OCompras")
        lcConsulta.AppendLine(" JOIN Articulos ON Articulos.Cod_Art = Renglones_OCompras.Cod_Art")
        lcConsulta.AppendLine(" CROSS JOIN (SELECT COUNT(*) AS numRows FROM Renglones_OCompras WHERE Documento = " & lcDocumento & ") Total")
        lcConsulta.AppendLine("WHERE Documento = " & lcDocumento)
        lcConsulta.AppendLine("GROUP BY Renglones_OCompras.Renglon, Renglones_OCompras.Cod_Art, Articulos.Nom_Art,  Renglones_OCompras.Can_Art1, Total.numRows, Renglones_OCompras.Caracter1")
        lcConsulta.AppendLine("")
        lcConsulta.AppendLine("")
        lcConsulta.AppendLine("")
        lcConsulta.AppendLine("")
        lcConsulta.AppendLine("")
        lcConsulta.AppendLine("")


        Dim loRenglones As DataTable = (New goDatos()).mObtenerTodosSinEsquema(lcConsulta.ToString(), "Renglones_OCompras").Tables(0)

        Dim loTabla As New DataTable("Renglones")

        loTabla.Columns.Add(New DataColumn("renglon", GetType(Integer)))
        loTabla.Columns.Add(New DataColumn("cod_art", GetType(String)))
        loTabla.Columns.Add(New DataColumn("nom_art", GetType(String)))
        loTabla.Columns.Add(New DataColumn("can_art", GetType(Decimal)))
        loTabla.Columns.Add(New DataColumn("art_alt", GetType(String)))

        For i As Integer = 0 To CDec(loRenglones.Rows(0).Item("Total")) - 1
            Dim loRenglon As DataRow = loTabla.NewRow()

            loRenglon("Renglon") = CDec(loRenglones.Rows(i).Item("Renglon"))
            loRenglon("cod_art") = CStr(loRenglones.Rows(i).Item("Cod_Art"))
            loRenglon("nom_art") = CStr(loRenglones.Rows(i).Item("Nom_Art"))
            loRenglon("can_art") = CDec(loRenglones.Rows(i).Item("Can_Art"))
            loRenglon("art_alt") = CStr(loRenglones.Rows(i).Item("Art_Alt"))

            loTabla.Rows.Add(loRenglon)
        Next

        Me.grdRenglones.poOrigenDeDatos = loTabla

        Me.grdRenglones.DataBind()
        Me.grdRenglones.mAlmacenarRenglones()
    End Sub

    Private Sub grdRenglones_mFilaSeleccionada(lnFilaAnterior As Integer, lnFilaNueva As Integer) Handles grdRenglones.mFilaSeleccionada
        Me.pcRenglonSelected = Me.grdRenglones.pnIndiceFilaSeleccionada + 1

        Me.lblAdvertencia.Text = "Se asignará la orden de compra " & Me.TxtBusqueda.pcTexto("Documento") & ", renglón " & Me.pcRenglonSelected & " como origen. "

        Me.mValidar()
    End Sub

#End Region

End Class
'-------------------------------------------------------------------------------------------'
' Fin del codigo																			'
'-------------------------------------------------------------------------------------------'
' KDE: 01/07/16: Codigo Inicial.								                            '
'-------------------------------------------------------------------------------------------'
' 
