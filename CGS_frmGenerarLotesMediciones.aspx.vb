'-------------------------------------------------------------------------------------------'
' Inicio del codigo
'-------------------------------------------------------------------------------------------'
Imports vis3Controles.wbcAdministradorMensajeModal
Imports Microsoft.VisualBasic
Imports cusAplicacion
Imports cusDatos
Imports System.Data

'-------------------------------------------------------------------------------------------'
' Inicio de clase "frmPlantilla"
'-------------------------------------------------------------------------------------------'
Partial Class CGS_frmGenerarLotesMediciones
    Inherits vis2formularios.frmFormularioGenerico

#Region "Declaraciones"
    Private Const KN_CANTIDAD_RENGLONES_LOTE As Integer = 5
#End Region

#Region "Propiedades"

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

    Private Property pcTablaDocumento() As String
        Get
            Return CStr(Me.ViewState("pcTablaDocumento"))
        End Get
        Set(ByVal value As String)
            Me.ViewState("pcTablaDocumento") = value
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
    Private Property pcOrigenArticulo() As String
        Get
            Return CStr(Me.ViewState("pcOrigenArticulo"))
        End Get
        Set(ByVal value As String)
            Me.ViewState("pcOrigenArticulo") = value
        End Set
    End Property
    Private Property pcOrigenAlmacen() As String
        Get
            Return CStr(Me.ViewState("pcOrigenAlmacen"))
        End Get
        Set(ByVal value As String)
            Me.ViewState("pcOrigenAlmacen") = value
        End Set
    End Property
    Private Property pcOrigenCantidad() As Decimal
        Get
            Return CDec(Me.ViewState("pcOrigenCantidad"))
        End Get
        Set(ByVal value As Decimal)
            Me.ViewState("pcOrigenCantidad") = value
        End Set
    End Property
#End Region

#Region "Eventos"

    Protected Sub mCargaPagina(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'La primera vez que se cargue el formulario...
        If Not Me.IsPostBack() Then

            Me.pnDecimalesParaCantidad = goOpciones.pnDecimalesParaCantidad
            'Me.txtTotal.pnNumeroDecimales = goOpciones.pnDecimalesParaCantidad

            Dim laParametros As Generic.Dictionary(Of String, Object)

            laParametros = Me.Session("frmComplementos.paParametros")
            Me.Session.Remove("frmComplementos.paParametros")

            If laParametros IsNot Nothing Then

                'Lee la colección de campos índice del formulario de origen
                Dim laIndices As Generic.Dictionary(Of String, Object)
                laIndices = laParametros("laIndices")

                Me.pcOrigenDocumento = CStr(laIndices("Documento")).Trim()
                Me.pcOrigenRenglon = CStr(laIndices("Renglon")).Trim()
                Me.pcTablaDocumento = CStr(laParametros("lcTabla")).Trim()


                Me.lblTitulo.Text = "Tabla: " & Me.pcTablaDocumento
                Me.lblRenglon.Text = " " & Me.pcOrigenRenglon
            End If
        End If

        Me.grdLotesMediciones.mActivarBotonAdicional(vis3Controles.grdListaRenglones.enuBotonesAdicionales.lnPrimerBoton, True, True)
        Me.grdLotesMediciones.pcSugerenciaBotonAdicional(vis3Controles.grdListaRenglones.enuBotonesAdicionales.lnPrimerBoton) = "Agregar 5 renglones"

        Me.grdLotesMediciones.mRegistrarColumna("cod_lot", "Lote / Colada", "", True, True, "String", False, 300)
        Me.grdLotesMediciones.mRegistrarColumna("can_lot", "Cantidad", 0D, True, True, "Decimal", False, 100)
        Me.grdLotesMediciones.mRegistrarColumna("can_pza", "Piezas", 0D, True, True, "Decimal", False, 100)
        Me.grdLotesMediciones.mRegistrarColumna("prc_des", "Porcentaje de Desperdicio", 0D, True, True, "Decimal", False, 100)
        Me.grdLotesMediciones.mRegistrarColumna("med_lng", "Longitud", 0D, True, True, "Decimal", False, 100)

        Me.grdLotesMediciones.mLimitarCampoTexto("cod_lot", True, 50)
        Me.grdLotesMediciones.pnDecimalesColumna("can_lot") = Me.pnDecimalesParaCantidad
        Me.grdLotesMediciones.pnDecimalesColumna("can_pza") = Me.pnDecimalesParaCantidad
        Me.grdLotesMediciones.pnDecimalesColumna("prc_des") = Me.pnDecimalesParaCantidad
        Me.grdLotesMediciones.pnDecimalesColumna("med_lng") = Me.pnDecimalesParaCantidad


        If Not Me.IsPostBack() Then

            If Me.pcTablaDocumento = "recepciones" Then
                Dim lcConsulta As String = "SELECT Documento FROM Renglones_Compras WHERE Tip_Ori = " & goServicios.mObtenerCampoFormatoSQL(Me.pcTablaDocumento) & " AND Doc_Ori = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento)

                Dim loTabla As DataTable = (New goDatos()).mObtenerTodosSinEsquema(lcConsulta, "Factura").Tables(0)

                If loTabla.Rows.Count > 0 Then
                    Dim lcFactura As String = CStr(loTabla.Rows(0).Item("Documento")).Trim()
                    Me.mMostrarMensajeModal("Operación no permitida", "Esta recepción ya está asociada a la factura de compra " & lcFactura & " y no se puede agregar mas lotes/coladas.", "a")
                    Me.cmdAceptar.Enabled = False
                    Return
                End If
            ElseIf Me.pcTablaDocumento = "entregas" Then
                Dim lcConsulta As String = "SELECT Documento FROM Renglones_Facturas WHERE Tip_Ori = " & goServicios.mObtenerCampoFormatoSQL(Me.pcTablaDocumento) & " AND Doc_Ori = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento)

                Dim loTabla As DataTable = (New goDatos()).mObtenerTodosSinEsquema(lcConsulta, "Factura").Tables(0)

                If loTabla.Rows.Count > 0 Then
                    Dim lcFactura As String = CStr(loTabla.Rows(0).Item("Documento")).Trim()
                    Me.mMostrarMensajeModal("Operación no permitida", "Esta entrega ya está asociada a la factura de venta " & lcFactura & " y no se puede agregar mas lotes/coladas.", "a")
                    Me.cmdAceptar.Enabled = False
                    Return
                End If
            End If

            Dim loConsulta As New StringBuilder()

            ''TRAER INFORMACIÓN DEL DOCUMENTO DE ACUERDO A LA TABLA DESDE DONDE SE EJECUTA EL COMPLEMENTO
            Select Case Me.pcTablaDocumento
                Case "recepciones"
                    loConsulta.AppendLine("SELECT   Recepciones.Status                AS Status,")
                    loConsulta.AppendLine("         Renglones_Recepciones.Cod_Art   AS Cod_Art,")
                    loConsulta.AppendLine("         Articulos.Nom_Art               AS Nom_Art,")
                    loConsulta.AppendLine("         Articulos.Usa_Lot               AS Usa_Lot,")
                    loConsulta.AppendLine("         Renglones_Recepciones.Cod_Alm   AS Cod_Alm,")
                    loConsulta.AppendLine("         Almacenes.Nom_Alm               AS Nom_Alm,")
                    loConsulta.AppendLine("         Renglones_Recepciones.Can_Art1  AS Can_Art")
                    loConsulta.AppendLine("FROM Renglones_Recepciones")
                    loConsulta.AppendLine(" JOIN Recepciones ON Recepciones.Documento = Renglones_Recepciones.Documento")
                    loConsulta.AppendLine(" JOIN Articulos ON Articulos.Cod_Art = Renglones_Recepciones.Cod_Art")
                    loConsulta.AppendLine(" JOIN Almacenes ON Almacenes.Cod_Alm = Renglones_Recepciones.Cod_Alm")
                    loConsulta.AppendLine("WHERE Renglones_Recepciones.Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento))
                    loConsulta.AppendLine(" AND Renglones_Recepciones.Renglon = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon))
                    loConsulta.AppendLine("")
                    loConsulta.AppendLine("")
                Case "renglones_traslados"
                    loConsulta.AppendLine("SELECT   Traslados.Status                AS Status,")
                    loConsulta.AppendLine("         Renglones_Traslados.Cod_Art     AS Cod_Art,")
                    loConsulta.AppendLine("         Articulos.Nom_Art               AS Nom_Art,")
                    loConsulta.AppendLine("         Articulos.Usa_Lot               AS Usa_Lot,")
                    loConsulta.AppendLine("         Traslados.Alm_Ori               AS Alm_Ori,")
                    loConsulta.AppendLine("         Traslados.Alm_Des               AS Alm_Des,")
                    loConsulta.AppendLine("         Origen.Nom_Alm                  AS Nom_AlmOri,")
                    loConsulta.AppendLine("         Destino.Nom_Alm                 AS Nom_AlmDes,")
                    loConsulta.AppendLine("         Renglones_Traslados.Can_Art1    AS Can_Art")
                    loConsulta.AppendLine("FROM Renglones_Traslados")
                    loConsulta.AppendLine(" JOIN Traslados ON Traslados.Documento = Renglones_Traslados.Documento")
                    loConsulta.AppendLine(" JOIN Articulos ON Articulos.Cod_Art = Renglones_Traslados.Cod_Art")
                    loConsulta.AppendLine(" JOIN Almacenes AS Origen ON Origen.Cod_Alm = Traslados.Alm_Ori")
                    loConsulta.AppendLine(" JOIN Almacenes AS Destino ON Destino.Cod_Alm = Traslados.Alm_Des")
                    loConsulta.AppendLine("WHERE Renglones_Traslados.Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento))
                    loConsulta.AppendLine(" AND Renglones_Traslados.Renglon = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon))
                    loConsulta.AppendLine("")
                    loConsulta.AppendLine("")
                Case "renglones_ajustes"
                    loConsulta.AppendLine("SELECT   Ajustes.Status              AS Status,")
                    loConsulta.AppendLine("         Renglones_Ajustes.Cod_Art   AS Cod_Art,")
                    loConsulta.AppendLine("         Articulos.Nom_Art           AS Nom_Art,")
                    loConsulta.AppendLine("         Articulos.Usa_Lot           AS Usa_Lot,")
                    loConsulta.AppendLine("         Renglones_Ajustes.Cod_Alm   AS Cod_Alm,")
                    loConsulta.AppendLine("         Almacenes.Nom_Alm           AS Nom_Alm,")
                    loConsulta.AppendLine("         Renglones_Ajustes.Can_Art1  AS Can_Art")
                    loConsulta.AppendLine("FROM Renglones_Ajustes")
                    loConsulta.AppendLine(" JOIN Ajustes ON Ajustes.Documento = Renglones_Ajustes.Documento")
                    loConsulta.AppendLine(" JOIN Articulos ON Articulos.Cod_Art = Renglones_Ajustes.Cod_Art")
                    loConsulta.AppendLine(" JOIN Almacenes ON Almacenes.Cod_Alm = Renglones_Ajustes.Cod_Alm")
                    loConsulta.AppendLine("WHERE Renglones_Ajustes.Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento))
                    loConsulta.AppendLine(" AND Renglones_Ajustes.Renglon = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon))
                    loConsulta.AppendLine("")
                    loConsulta.AppendLine("")
                Case "entregas"
                    loConsulta.AppendLine("SELECT   Entregas.Status             AS Status,")
                    loConsulta.AppendLine("         Renglones_Entregas.Cod_Art  AS Cod_Art,")
                    loConsulta.AppendLine("         Articulos.Nom_Art           AS Nom_Art,")
                    loConsulta.AppendLine("         Articulos.Usa_Lot           AS Usa_Lot,")
                    loConsulta.AppendLine("         Renglones_Entregas.Cod_Alm  AS Cod_Alm,")
                    loConsulta.AppendLine("         Almacenes.Nom_Alm           AS Nom_Alm,")
                    loConsulta.AppendLine("         Renglones_Entregas.Can_Art1 AS Can_Art")
                    loConsulta.AppendLine("FROM Renglones_Entregas")
                    loConsulta.AppendLine(" JOIN Entregas ON Entregas.Documento = Renglones_Entregas.Documento")
                    loConsulta.AppendLine(" JOIN Articulos ON Articulos.Cod_Art = Renglones_Entregas.Cod_Art")
                    loConsulta.AppendLine(" JOIN Almacenes ON Almacenes.Cod_Alm = Renglones_Entregas.Cod_Alm")
                    loConsulta.AppendLine("WHERE Renglones_Entregas.Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento))
                    loConsulta.AppendLine(" AND Renglones_Entregas.Renglon = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon))
                    loConsulta.AppendLine("")
                    loConsulta.AppendLine("")
            End Select

            Dim loRenglones As DataTable = (New goDatos()).mObtenerTodosSinEsquema(loConsulta.ToString(), "Renglones_Recepciones").Tables(0)

            Me.pcOrigenArticulo = CStr(loRenglones.Rows(0).Item("Cod_Art")).Trim()

            If Me.pcTablaDocumento = "renglones_traslados" Then
                Me.pcOrigenAlmacen = CStr(loRenglones.Rows(0).Item("Alm_Ori")).Trim()
            Else
                Me.pcOrigenAlmacen = CStr(loRenglones.Rows(0).Item("Cod_Alm")).Trim()
            End If

            Me.pcOrigenCantidad = CDec(loRenglones.Rows(0).Item("Can_Art"))


            Me.lblArticulo.Text = Me.pcOrigenArticulo & ":  " & CStr(loRenglones.Rows(0).Item("Nom_Art")).Trim()
            If Me.pcTablaDocumento = "renglones_traslados" Then
                Me.lblDAlmacen.Text = "Almacenes:"
                Me.lblAlmacen.Text = Me.pcOrigenAlmacen & ":  " & CStr(loRenglones.Rows(0).Item("Nom_AlmOri")).Trim() & " - " & CStr(loRenglones.Rows(0).Item("Alm_Des")).Trim() & ": " & CStr(loRenglones.Rows(0).Item("Nom_AlmDes")).Trim()
            Else
                Me.lblAlmacen.Text = Me.pcOrigenAlmacen & ":  " & CStr(loRenglones.Rows(0).Item("Nom_Alm")).Trim()
            End If

            Me.lblCantidad.Text = goServicios.mObtenerFormatoCadena(Me.pcOrigenCantidad, goServicios.enuOpcionesRedondeo.KN_RedondeoPuntoMedio, Me.pnDecimalesParaCantidad)

            Me.mCargarTablaVacia()

            If CBool(loRenglones.Rows(0).Item("Usa_lot")) = False Then
                Me.mMostrarMensajeModal("Operación NO permitida", "El artículo no maneja lotes.", "e", False)
                Me.cmdAceptar.Enabled = False
                Me.cmdCancelar.Text = "Cerrar"
            ElseIf CStr(loRenglones.Rows(0).Item("Status")).Trim() <> "Pendiente" Then
                Me.mMostrarMensajeModal("Operación NO permitida", "El documento debe estar en estatus Pendiente.", "e", False)
                Me.cmdAceptar.Enabled = False
                Me.cmdCancelar.Text = "Cerrar"
            End If

        Else

            Me.grdLotesMediciones.DataBind()

        End If

        Me.grdLotesMediciones.mHabilitarBotonera(True)
        Me.grdLotesMediciones.mAlmacenarRenglones()

    End Sub

    Protected Sub grdLotesMediciones_mClicBotonAdicional(lnBoton As vis3Controles.grdListaRenglones.enuBotonesAdicionales) Handles grdLotesMediciones.mClicBotonAdicional

        'AGREGAR UNA FILA AL GRID
        Select Case lnBoton
            Case vis3Controles.grdListaRenglones.enuBotonesAdicionales.lnPrimerBoton


                Dim loTabla As DataTable = Me.grdLotesMediciones.poOrigenDeDatos

                For i As Integer = 1 To KN_CANTIDAD_RENGLONES_LOTE
                    Dim loRenglon As DataRow = loTabla.NewRow()

                    loRenglon("Renglon") = loTabla.Rows.Count + 1
                    loRenglon("cod_lot") = ""
                    loRenglon("can_lot") = 0D
                    loRenglon("can_pza") = 0D
                    loRenglon("prc_des") = 0D
                    loRenglon("med_lng") = 0D

                    loTabla.Rows.Add(loRenglon)
                Next

                Me.grdLotesMediciones.DataBind()
                Me.grdLotesMediciones.mAlmacenarRenglones()
        End Select

    End Sub

    Protected Sub cmdAceptar_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles cmdAceptar.Click

        If Not Me.mDatosValidos() Then
            Return
        End If

        Me.mGenerarLotesMediciones(Me.grdLotesMediciones.poOrigenDeDatos)

    End Sub

    Protected Sub cmdLimpiar_Click(sender As Object, e As EventArgs) Handles cmdLimpiar.Click

        Me.mCargarTablaVacia()

    End Sub

#End Region

#Region "Metodos"


    ''' <summary>
    ''' Carga la tabla inicial en blanco para el grid de artículos.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub mCargarTablaVacia()

        Dim loTabla As New DataTable("Articulos")

        loTabla.Columns.Add(New DataColumn("renglon", GetType(Integer)))
        loTabla.Columns.Add(New DataColumn("cod_lot", GetType(String)))
        loTabla.Columns.Add(New DataColumn("can_lot", GetType(Decimal)))
        loTabla.Columns.Add(New DataColumn("can_pza", GetType(Decimal)))
        loTabla.Columns.Add(New DataColumn("prc_des", GetType(Decimal)))
        loTabla.Columns.Add(New DataColumn("med_lng", GetType(Decimal)))

        For i As Integer = 1 To KN_CANTIDAD_RENGLONES_LOTE
            Dim loRenglon As DataRow = loTabla.NewRow()

            loRenglon("Renglon") = i
            loRenglon("cod_lot") = ""
            loRenglon("can_lot") = 0D
            loRenglon("can_pza") = 0D
            loRenglon("prc_des") = 0D
            loRenglon("med_lng") = 0D

            loTabla.Rows.Add(loRenglon)
        Next

        Me.grdLotesMediciones.poOrigenDeDatos = loTabla

        Me.grdLotesMediciones.DataBind()
        Me.grdLotesMediciones.mAlmacenarRenglones()

    End Sub

    ''' <summary>
    ''' Valida los datos de los renglones y devuelve true si sin válidos, y false en caso contrario.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function mDatosValidos() As Boolean

        Dim laVacios As New Generic.List(Of Integer)
        Dim lnTotalCantidad As Decimal = 0D

        Dim loMensaje As New StringBuilder()

        For Each loRenglon As DataRow In grdLotesMediciones.poOrigenDeDatos.Rows

            Dim lnRenglon As Integer = CInt(loRenglon("Renglon"))
            Dim lcLote As String = CStr(loRenglon("cod_lot")).Trim()
            Dim lnCantidad As Decimal = CDec(loRenglon("can_lot"))
            Dim lnPiezas As Decimal = CDec(loRenglon("can_pza"))
            Dim lnPorcentaje As Decimal = CDec(loRenglon("prc_des"))
            Dim lnLongitud As Decimal = CDec(loRenglon("med_lng"))

            'SI NO INDICA EL ARTÍCULO EL RENGLÓN NO SE GUARDA
            If (lcLote = "") Then
                laVacios.Add(lnRenglon)
                Continue For
            End If

            lnTotalCantidad += lnCantidad

        Next loRenglon

        If loMensaje.Length > 0 Then
            Me.mMostrarMensajeModal("Datos no Válidos", loMensaje.ToString(), "a", True)
            Return False
        End If

        If lnTotalCantidad <> Me.pcOrigenCantidad Then
            Me.mMostrarMensajeModal("Datos no Válidos", "La cantidad total en los lotes/coladas ingresados (" & CStr(lnTotalCantidad) & ") debe coincidir con la cantidad del renglón (" & goServicios.mObtenerFormatoCadena(Me.pcOrigenCantidad, goServicios.enuOpcionesRedondeo.KN_RedondeoPuntoMedio, Me.pnDecimalesParaCantidad) & ").", "a", False)
            Return False
        End If

        If (Me.grdLotesMediciones.pnTotalFilas = laVacios.Count) Then
            Me.mMostrarMensajeModal("No hay Lotes o Coladas", "Debe indicar al menos un renglón con un lote o coladas para generar. " & _
                                    "Los renglones que no tengan nombre serán eliminados.", "a", False)
            Return False
        End If

        'ELIMINA LOS RENGLONES VACÍOS (SIN NOMBRE DE ARTÍCULOS) DESDE EL ÚLTIMO AL PRIMERO
        laVacios.Sort()
        laVacios.Reverse()

        For Each lnValor As Integer In laVacios
            Me.grdLotesMediciones.mEliminarRenglon(lnValor - 1, False, False)
        Next

        If (laVacios.Count > 0) Then
            Me.grdLotesMediciones.DataBind()
        End If
        Me.grdLotesMediciones.mAlmacenarRenglones()

        Return True

    End Function

    ''' <summary>
    ''' Genera e inserta los artículos según los datos ingresados por el usuario
    ''' </summary>
    ''' <param name="loRenglones"></param>
    ''' <remarks></remarks>
    Private Sub mGenerarLotesMediciones(loRenglones As DataTable)

        Dim loConsulta As New StringBuilder()

        'Dim lcDocumento As String =
        'Dim lcRenglonOrigen As String = goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon)

        loConsulta.AppendLine("DECLARE @lcDocumento AS VARCHAR(10) = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento))
        loConsulta.AppendLine("DECLARE @lnRenglonOrigen AS INT = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon))
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lcCod_Art AS VARCHAR(8) = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenArticulo))
        loConsulta.AppendLine("DECLARE @lcCod_Alm AS VARCHAR(10) = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenAlmacen))
        loConsulta.AppendLine("DECLARE @lcTabla AS VARCHAR(30) = " & goServicios.mObtenerCampoFormatoSQL(Me.pcTablaDocumento))
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SET @lcTabla = CASE RTRIM(@lcTabla)")
        loConsulta.AppendLine("					WHEN 'renglones_ajustes' THEN 'Ajustes_Inventarios'")
        loConsulta.AppendLine("					WHEN 'renglones_traslados' THEN 'Traslados'")
        loConsulta.AppendLine("					ELSE @lcTabla ")
        loConsulta.AppendLine("				END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lcLote AS VARCHAR(30) ")
        loConsulta.AppendLine("DECLARE @lnCan_Lote AS DECIMAL(28,2)")
        loConsulta.AppendLine("DECLARE @lnCan_Piezas AS DECIMAL(28,2)")
        loConsulta.AppendLine("DECLARE @lnPorc_Desp AS DECIMAL(28,2)")
        loConsulta.AppendLine("DECLARE @lnLongitud AS DECIMAL(28,2) ")
        loConsulta.AppendLine("DECLARE @lcAdicional AS VARCHAR(30)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @RC INT")
        loConsulta.AppendLine("DECLARE @lcProximoContador VARCHAR(10)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lnDisponible AS DECIMAL(28,2)")
        loConsulta.AppendLine("DECLARE @llValido AS BIT = 0")
        loConsulta.AppendLine("DECLARE @lcMensaje AS VARCHAR(MAX) = ''")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lcUsuario CHAR(10) = " & goServicios.mObtenerCampoFormatoSQL(goUsuario.pcCodigo) & ";")
        loConsulta.AppendLine("DECLARE @lcEquipo CHAR(30) = " & goServicios.mObtenerCampoFormatoSQL(goAuditoria.pcNombreEquipo) & ";")
        loConsulta.AppendLine("DECLARE @lcSucursal CHAR(10) = " & goServicios.mObtenerCampoFormatoSQL(goSucursal.pcCodigo) & ";")
        loConsulta.AppendLine("")
        '--TABLA TEMPORAL PARA TENER LA INFORMACIÓN DE LOS LOTES Y MEDICIONES QUE SE VAN A GUARDAR
        loConsulta.AppendLine("CREATE TABLE #tmpLoteMediciones (Renglon INT,")
        loConsulta.AppendLine("								 Contador VARCHAR(10),")
        loConsulta.AppendLine("								 Lote VARCHAR(30),")
        loConsulta.AppendLine("								 Cantidad_Lote DECIMAL(28,2),")
        loConsulta.AppendLine("								 Piezas DECIMAL(28,2),")
        loConsulta.AppendLine("								 Porc_Desperdicio DECIMAL(28,2),")
        loConsulta.AppendLine("								 Longitud DECIMAL (28,2),")
        loConsulta.AppendLine("								 Adicional VARCHAR(30)")
        loConsulta.AppendLine(")")
        loConsulta.AppendLine("")

        For Each loRenglon As DataRow In loRenglones.Rows

            Dim lnRenglon As Integer = CInt(loRenglon("Renglon"))

            '--INSERTAR LOS DATOS DEL GRID
            loConsulta.Append("INSERT INTO #tmpLoteMediciones VALUES (")
            loConsulta.Append(lnRenglon.ToString()) 'RENGLÓN
            loConsulta.Append(", ")
            loConsulta.Append("0")                  'CONTADOR
            loConsulta.Append(", ")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("cod_lot")))) 'LOTE
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("can_lot")))) 'CANTIDAD LOTE
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("can_pza")))) 'PIEZAS
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("prc_des")))) 'PORCENTAJE DE DESPERDICIO
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("med_lng")))) 'LONGITUD
            loConsulta.Append(",''")  'ADICIONAL
            loConsulta.AppendLine(");")

        Next loRenglon

        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lnFilas AS INT = (SELECT MAX(Renglon) FROM #tmpLoteMediciones)")
        loConsulta.AppendLine("DECLARE @lnRenglon AS INT = 1")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("WHILE (@lnRenglon <= @lnFilas)")
        loConsulta.AppendLine("BEGIN ")
        loConsulta.AppendLine("")
        loConsulta.AppendLine(" SELECT  @lcLote = Lote,")
        loConsulta.AppendLine("		    @lnCan_Lote = Cantidad_Lote,")
        loConsulta.AppendLine("		    @lnCan_Piezas = Piezas,")
        loConsulta.AppendLine("		    @lnPorc_Desp = Porc_Desperdicio,")
        loConsulta.AppendLine("		    @lnLongitud = Longitud,")
        loConsulta.AppendLine("		    @lcAdicional = Adicional")
        loConsulta.AppendLine(" FROM #tmpLoteMediciones")
        loConsulta.AppendLine(" WHERE Renglon = @lnRenglon")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	SET @lcAdicional = CASE @lcTabla ")
        loConsulta.AppendLine("						WHEN 'Ajustes_Inventarios' THEN (SELECT Tipo FROM Renglones_Ajustes WHERE Documento = @lcDocumento AND Renglon = @lnRenglonOrigen)")
        loConsulta.AppendLine("						WHEN 'Traslados' THEN (SELECT Alm_Des FROM Traslados WHERE Documento =  @lcDocumento)")
        loConsulta.AppendLine("					  END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	IF @lcAdicional = 'Salida' OR @lcTabla = 'Traslados' OR @lcTabla = 'Entregas'")
        loConsulta.AppendLine("	BEGIN")
        loConsulta.AppendLine("		SET @lnDisponible = COALESCE((SELECT Exi_Act1 FROM Renglones_Lotes ")
        loConsulta.AppendLine("								WHERE Cod_Art = @lcCod_Art AND Cod_Lot = @lcLote AND Cod_Alm = @lcCod_Alm ),0)")
        loConsulta.AppendLine("		")
        loConsulta.AppendLine("		IF(@lnDisponible > 0 AND @lnCan_Lote <= @lnDisponible)")
        loConsulta.AppendLine("			SET @llValido = 1")
        loConsulta.AppendLine("		ELSE")
        loConsulta.AppendLine("			SET @lcMensaje = @lcMensaje + '- ' + @lcLote + '. Disponible: ' + CAST(@lnDisponible AS CHAR) + CHAR(13)")
        loConsulta.AppendLine("	END")
        loConsulta.AppendLine("	ELSE")
        loConsulta.AppendLine("		SET @llValido = 1")
        loConsulta.AppendLine("")
        '--INSERCIÓN EN OPERACIONES LOTES
        loConsulta.AppendLine("	IF @llValido = 1")
        loConsulta.AppendLine("	BEGIN")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	    IF @lnCan_Lote <> 0 ")
        loConsulta.AppendLine("	    BEGIN")
        loConsulta.AppendLine("	    	IF (@lcTabla = 'Recepciones')")
        loConsulta.AppendLine("	    	BEGIN	")
        loConsulta.AppendLine("	    		INSERT INTO Operaciones_Lotes (Cod_Alm, Cod_Art, Cod_Lot, Cantidad, Num_Doc, Renglon, Tip_Doc, Tip_Ope, Ren_Ori)")
        loConsulta.AppendLine("	    		VALUES (@lcCod_Alm, @lcCod_Art, @lcLote, @lnCan_Lote,@lcDocumento,@lnRenglon,@lcTabla,'Entrada',@lnRenglonOrigen)")
        loConsulta.AppendLine("	    	END		")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	    	IF @lcTabla = 'Ajustes_Inventarios'")
        loConsulta.AppendLine("	    	BEGIN")
        loConsulta.AppendLine("	    		INSERT INTO Operaciones_Lotes (Cod_Alm, Cod_Art, Cod_Lot, Cantidad, Num_Doc, Renglon, Tip_Doc, Tip_Ope, Ren_Ori)")
        loConsulta.AppendLine("	    		VALUES (@lcCod_Alm, @lcCod_Art, @lcLote, @lnCan_Lote,@lcDocumento,@lnRenglon,@lcTabla,@lcAdicional,@lnRenglonOrigen)")
        loConsulta.AppendLine("	    	END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	    	IF @lcTabla = 'Traslados'")
        loConsulta.AppendLine("	    	BEGIN")
        loConsulta.AppendLine("	    		----SALIDA DEL ARTÍCULO EN ALMACÉN DE ORIGEN			")
        loConsulta.AppendLine("	    		INSERT INTO Operaciones_Lotes (Cod_Alm, Cod_Art, Cod_Lot, Cantidad, Num_Doc, Renglon, Tip_Doc, Tip_Ope, Ren_Ori)")
        loConsulta.AppendLine("	    		VALUES (@lcCod_Alm, @lcCod_Art, @lcLote, @lnCan_Lote,@lcDocumento,1,@lcTabla,'Salida',@lnRenglonOrigen)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	    		----ENTRADA DEL ARTÍCULO EN EL ALMACÉN DESTINO")
        loConsulta.AppendLine("	    		INSERT INTO Operaciones_Lotes (Cod_Alm, Cod_Art, Cod_Lot, Cantidad, Num_Doc, Renglon, Tip_Doc, Tip_Ope, Ren_Ori)")
        loConsulta.AppendLine("	    		VALUES (@lcAdicional, @lcCod_Art, @lcLote, @lnCan_Lote,@lcDocumento,2,@lcTabla,'Entrada',@lnRenglonOrigen)")
        loConsulta.AppendLine("	    	END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	    	IF @lcTabla = 'Entregas'")
        loConsulta.AppendLine("	    		INSERT INTO Operaciones_Lotes (Cod_Alm, Cod_Art, Cod_Lot, Cantidad, Num_Doc, Renglon, Tip_Doc, Tip_Ope, Ren_Ori)")
        loConsulta.AppendLine("	    		VALUES (@lcCod_Alm, @lcCod_Art, @lcLote, @lnCan_Lote,@lcDocumento,1,@lcTabla,'Salida',@lnRenglonOrigen)")
        loConsulta.AppendLine("	    	END")
        loConsulta.AppendLine("	    ")
        loConsulta.AppendLine("	    END")
        loConsulta.AppendLine("")
        '--OBTENER PRÓXIMO CONTADOR PARA INSERTAR LA MEDICIÓN
        loConsulta.AppendLine("	    IF @lnCan_Piezas <> 0 OR @lnPorc_Desp <> 0 OR @lnLongitud <> 0")
        loConsulta.AppendLine("	    BEGIN")
        loConsulta.AppendLine("	    	EXECUTE @RC = [dbo].[mObtenerProximoContador] ")
        loConsulta.AppendLine("	    	'Mediciones'")
        loConsulta.AppendLine("	    	,@lcSucursal")
        loConsulta.AppendLine("	    	,'Normal'")
        loConsulta.AppendLine("	    	,@lcProximoContador OUTPUT")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	    	UPDATE #tmpLoteMediciones SET Contador = @lcProximoContador WHERE Renglon = @lnRenglon")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	    	INSERT INTO Mediciones (Documento, Adicional, Status, Posicion, ")
        loConsulta.AppendLine("	    							Cod_Art, Cod_Alm, Cod_Reg, Tip_Med, Num_Lot, Origen, Cod_Suc, Ren_Ori,")
        loConsulta.AppendLine("	    							Prioridad, Usu_Cre, Usu_Mod, Equ_Cre, Equ_Mod)")
        loConsulta.AppendLine("            VALUES(@lcProximoContador, 'LOTE|'+@lcCod_Art+'|'+@lcCod_Alm+'|'+@lcLote+ ")
        loConsulta.AppendLine("	    		CASE WHEN @lcTabla = 'Recepciones' THEN '|Entrada|'")
        loConsulta.AppendLine("	    			WHEN (@lcTabla = 'Traslados' OR @lcTabla = 'Entregas') THEN '|Salida|'")
        loConsulta.AppendLine("	    			WHEN @lcTabla = 'Ajustes_Inventarios' THEN (CASE WHEN @lcAdicional = 'Entrada' THEN '|Entrada|' ELSE '|Salida|' END)")
        loConsulta.AppendLine("	    		END")
        loConsulta.AppendLine("	    		+ CAST(@lnRenglonOrigen AS CHAR), 'Pendiente', 'Por Iniciar Medicion', ")
        loConsulta.AppendLine("            	@lcCod_Art, @lcCod_Alm,@lcDocumento, 'Prueba',@lcLote, @lcTabla, @lcSucursal, ")
        loConsulta.AppendLine("	    		(SELECT TOP 1 CAST(Renglon AS CHAR) FROM #tmpLoteMediciones WHERE Lote = @lcLote ORDER BY Renglon ASC),")
        loConsulta.AppendLine("            	'Media', @lcUsuario, @lcUsuario, @lcEquipo, @lcEquipo)")
        loConsulta.AppendLine("")
        '--INSERCIÓN DE LOS RENGLONES DE LAS MEDICIONES SEGÚN LA TABLA DESDE DONDE SE EJECUTÓ EL COMPLEMENTO
        loConsulta.AppendLine("		    IF RTRIM(@lcTabla) = 'Recepciones'")
        loConsulta.AppendLine("		    BEGIN")
        loConsulta.AppendLine("		    	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("		    	VALUES (@lcProximoContador, 1, 'NREC-NPIEZ', 'NÚMERO DE PIEZAS NOTAS DE RECEPCIÓN','UND', 1, 99999, @lnCan_Piezas, ")
        loConsulta.AppendLine("		    			CASE WHEN @lnCan_Piezas = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("		    	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("		    	VALUES (@lcProximoContador, 2, 'NREC-PDESP', 'PORCENTAJE DE DESPERDICIO NOTAS DE RECEPCIÓN','NA', 0, 99, @lnPorc_Desp, ")
        loConsulta.AppendLine("		    			CASE WHEN @lnPorc_Desp = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("		    END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("		    IF RTRIM(@lcTabla) = 'Traslados'")
        loConsulta.AppendLine("		    BEGIN")
        loConsulta.AppendLine("		    	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("		    	VALUES (@lcProximoContador, 1, 'TA-NPIEZ', 'NÚMERO DE PIEZAS TRASLADOS','UND', 1, 99999, @lnCan_Piezas, ")
        loConsulta.AppendLine("		    			CASE WHEN @lnCan_Piezas = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("		    	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("		    	VALUES (@lcProximoContador, 2, 'TA-PDESP', 'PORCENTAJE DE DESPERDICIO TRASLADOS','NA', 0, 99, @lnPorc_Desp, ")
        loConsulta.AppendLine("		    			CASE WHEN @lnPorc_Desp = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("		    	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("		    	VALUES (@lcProximoContador, 3, 'TA-LARG', 'LARGO REAL / TRASLADOS','MTR', 1, 99, @lnLongitud, ")
        loConsulta.AppendLine("		    			CASE WHEN @lnLongitud = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("		    END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("		    IF RTRIM(@lcTabla) = 'Ajustes_Inventarios'")
        loConsulta.AppendLine("		    BEGIN")
        loConsulta.AppendLine("		    	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("		    	VALUES (@lcProximoContador, 1, 'AINV-NPIEZ', 'NÚMERO DE PIEZAS AJUSTES DE INVENTARIO','UND', 1, 99999, @lnCan_Piezas, ")
        loConsulta.AppendLine("		    			CASE WHEN @lnCan_Piezas = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("		    	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("		    	VALUES (@lcProximoContador, 2, 'AINV-PDESP', 'PORCENTAJE DE DESPERDICIO AJUSTES DE INVENTARIO','NA', 0, 99, @lnPorc_Desp, ")
        loConsulta.AppendLine("		    			CASE WHEN @lnPorc_Desp = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("		    	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("		    	VALUES (@lcProximoContador, 3, 'AINV-LARG', 'LARGO REAL / AJUSTES DE INVENTARIO','MTR', 1, 99, @lnLongitud, ")
        loConsulta.AppendLine("		    			CASE WHEN @lnLongitud = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("		    END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("		    IF RTRIM(@lcTabla) = 'Entregas'")
        loConsulta.AppendLine("		    BEGIN")
        loConsulta.AppendLine("		    	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("		    	VALUES (@lcProximoContador, 1, 'NENT-NPIEZ', 'NÚMERO DE PIEZAS NOTAS DE ENTREGA','UND', 1, 99999, @lnCan_Piezas, ")
        loConsulta.AppendLine("		    			CASE WHEN @lnCan_Piezas = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("		    	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("		    	VALUES (@lcProximoContador, 2, 'NENT-PDESP', 'PORCENTAJE DE DESPERDICIO NOTAS DE ENTREGA','NA', 0, 99, @lnPorc_Desp, ")
        loConsulta.AppendLine("		    			CASE WHEN @lnPorc_Desp = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("		    	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("		    	VALUES (@lcProximoContador, 3, 'NENT-LARG', 'LARGO REAL / NOTAS DE ENTREGA','MTR', 1, 99, @lnLongitud, ")
        loConsulta.AppendLine("		    			CASE WHEN @lnLongitud = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("		    END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine(" END")
        loConsulta.AppendLine("	SET @lnRenglon = @lnRenglon + 1")
        loConsulta.AppendLine("END ")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SELECT @llValido AS Valido, @lcMensaje AS Mensaje")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DROP TABLE #tmpLoteMediciones")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")

        Dim lodatos As New goDatos()

        Dim loTabla As DataTable

        Try
            Dim laSentencias As New ArrayList()
            'laSentencias.Add(loConsulta.ToString())

            'lodatos.mEjecutarTransaccion(laSentencias)
            loTabla = lodatos.mObtenerTodosSinEsquema(loConsulta.ToString(), "Mensaje").Tables(0)

            Me.mCargarTablaVacia()

            If CBool(loTabla.Rows(0).Item("Valido")) = True Then

                Me.mMostrarMensajeModal("Lotes/Coladas y Mediciones Generados", "Se generó un total de " & loRenglones.Rows.Count & " Lotes. ", "i", False)
            Else
                Me.mMostrarMensajeModal("Operación No Permitida", "Los siguientes lotes no tienen disponible la cantidad indicada: <br/>" & CStr(loTabla.Rows(0).Item("Mensaje")).Trim() & ".", "i", False)
            End If

        Catch ex As Exception

            Dim lcMensaje As String = ex.Message
            Dim lnDesde As Integer = lcMensaje.IndexOf("Los siguientes lotes o coladas")
            Dim lnHasta As Integer = lcMensaje.IndexOf("Número de Transacción") - 1

            If (lnHasta > lnDesde) Then
                lcMensaje = lcMensaje.Substring(lnDesde, lnHasta - lnDesde)
            End If

            Me.mMostrarMensajeModal("Proceso no completado", "No fue posible generar los artículos. Información Adicional:<br/> " & lcMensaje, "e", True)

        End Try


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

#End Region



End Class
'-------------------------------------------------------------------------------------------'
' Fin del codigo																			'
' 
