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

    Private Property plSoloLectura As Boolean 
        Get 
            Return CBool(Me.ViewState("plSoloLectura"))
        End Get
        Set(value As Boolean)
            Me.ViewState("plSoloLectura") = value
        End Set
    End Property

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

            Me.mInicializar()
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

        Dim llEditable As Boolean = Not Me.plSoloLectura

        Me.grdLotesMediciones.mActivarBotonAdicional(vis3Controles.grdListaRenglones.enuBotonesAdicionales.lnPrimerBoton, llEditable, llEditable)
        Me.grdLotesMediciones.pcSugerenciaBotonAdicional(vis3Controles.grdListaRenglones.enuBotonesAdicionales.lnPrimerBoton) = "Agregar 5 renglones"

        Me.grdLotesMediciones.mRegistrarColumna("cod_lot", "Lote / Colada", "", True, llEditable, "String", False, 300)
        Me.grdLotesMediciones.mRegistrarColumna("can_lot", "Cantidad", 0D, True, llEditable, "Decimal", False, 100)
        Me.grdLotesMediciones.mRegistrarColumna("can_pza", "Piezas", 0D, True, llEditable, "Decimal", False, 100)
        Me.grdLotesMediciones.mRegistrarColumna("prc_des", "Porcentaje de Desperdicio", 0D, True, llEditable, "Decimal", False, 100)
        Me.grdLotesMediciones.mRegistrarColumna("med_lng", "Longitud", 0D, True, llEditable, "Decimal", False, 100)

        Me.grdLotesMediciones.mLimitarCampoTexto("cod_lot", True, 50)
        Me.grdLotesMediciones.pnDecimalesColumna("can_lot") = Me.pnDecimalesParaCantidad
        Me.grdLotesMediciones.pnDecimalesColumna("can_pza") = Me.pnDecimalesParaCantidad
        Me.grdLotesMediciones.pnDecimalesColumna("prc_des") = Me.pnDecimalesParaCantidad
        Me.grdLotesMediciones.pnDecimalesColumna("med_lng") = Me.pnDecimalesParaCantidad


        If Not Me.IsPostBack() Then

            If Me.pcTablaDocumento = "recepciones" Then
                Dim lcConsulta As String = "SELECT Documento FROM Renglones_Facturas WHERE Tip_Ori = " & goServicios.mObtenerCampoFormatoSQL(Me.pcTablaDocumento) & " AND Doc_Ori = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento)

                Dim loTabla As DataTable = (New goDatos()).mObtenerTodosSinEsquema(lcConsulta, "Factura").Tables(0)

                If loTabla.Rows.Count > 0 Then
                    Dim lcFactura As String = CStr(loTabla.Rows(0).Item("Documento")).Trim()
                    Me.mMostrarMensajeModal("Operación no permitida", "Esta recepción ya está asociada a la factura de compra " & lcFactura & " y no se puede agregar mas lotes/coladas.", "a")
                    Me.cmdAceptar.Enabled = False
                    Return
                End If
            End If

            Dim loConsulta As New StringBuilder()

            Select Case Me.pcTablaDocumento
                Case "recepciones"
                    loConsulta.AppendLine("SELECT   Renglones_Recepciones.Cod_Art   AS Cod_Art,")
                    loConsulta.AppendLine("         Articulos.Nom_Art               AS Nom_Art,")
                    loConsulta.AppendLine("         Renglones_Recepciones.Cod_Alm   AS Cod_Alm,")
                    loConsulta.AppendLine("         Almacenes.Nom_Alm               AS Nom_Alm,")
                    loConsulta.AppendLine("         Renglones_Recepciones.Can_Art1  AS Can_Art")
                    loConsulta.AppendLine("FROM Renglones_Recepciones")
                    loConsulta.AppendLine(" JOIN Articulos ON Articulos.Cod_Art = Renglones_Recepciones.Cod_Art")
                    loConsulta.AppendLine(" JOIN Almacenes ON Almacenes.Cod_Alm = Renglones_Recepciones.Cod_Alm")
                    loConsulta.AppendLine("WHERE Renglones_Recepciones.Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento))
                    loConsulta.AppendLine(" AND Renglones_Recepciones.Renglon = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon))
                    loConsulta.AppendLine("")
                    loConsulta.AppendLine("")
                Case "traslados"
                    loConsulta.AppendLine("SELECT   Renglones_Traslados.Cod_Art     AS Cod_Art,")
                    loConsulta.AppendLine("         Articulos.Nom_Art               AS Nom_Art,")
                    loConsulta.AppendLine("         Renglones_Traslados.Cod_Alm     AS Cod_Alm,")
                    loConsulta.AppendLine("         Almacenes.Nom_Alm               AS Nom_Alm,")
                    loConsulta.AppendLine("         Renglones_Traslados.Can_Art1    AS Can_Art")
                    loConsulta.AppendLine("FROM Renglones_Traslados")
                    loConsulta.AppendLine(" JOIN Articulos ON Articulos.Cod_Art = Renglones_Traslados.Cod_Art")
                    loConsulta.AppendLine(" JOIN Almacenes ON Almacenes.Cod_Alm = Renglones_Traslados.Cod_Alm")
                    loConsulta.AppendLine("WHERE Renglones_Traslados.Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento))
                    loConsulta.AppendLine(" AND Renglones_Traslados.Renglon = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon))
                    loConsulta.AppendLine("")
                    loConsulta.AppendLine("")
                Case "ajustes"
                    loConsulta.AppendLine("SELECT   Renglones_Ajustes.Cod_Art   AS Cod_Art,")
                    loConsulta.AppendLine("         Articulos.Nom_Art           AS Nom_Art,")
                    loConsulta.AppendLine("         Renglones_Ajustes.Cod_Alm   AS Cod_Alm,")
                    loConsulta.AppendLine("         Almacenes.Nom_Alm           AS Nom_Alm,")
                    loConsulta.AppendLine("         Renglones_Ajustes.Can_Art1  AS Can_Art")
                    loConsulta.AppendLine("FROM Renglones_Ajustes")
                    loConsulta.AppendLine(" JOIN Articulos ON Articulos.Cod_Art = Renglones_Ajustes.Cod_Art")
                    loConsulta.AppendLine(" JOIN Almacenes ON Almacenes.Cod_Alm = Renglones_Ajustes.Cod_Alm")
                    loConsulta.AppendLine("WHERE Renglones_Ajustes.Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento))
                    loConsulta.AppendLine(" AND Renglones_Ajustes.Renglon = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon))
                    loConsulta.AppendLine("")
                    loConsulta.AppendLine("")
            End Select

            Dim loRenglones As DataTable = (New goDatos()).mObtenerTodosSinEsquema(loConsulta.ToString(), "Renglones_Recepciones").Tables(0)

            Me.pcOrigenArticulo = CStr(loRenglones.Rows(0).Item("Cod_Art")).Trim()
            Me.pcOrigenAlmacen = CStr(loRenglones.Rows(0).Item("Cod_Alm")).Trim()
            Me.pcOrigenCantidad = CDec(loRenglones.Rows(0).Item("Can_Art"))


            Me.lblArticulo.Text = Me.pcOrigenArticulo & ":  " & CStr(loRenglones.Rows(0).Item("Nom_Art")).Trim()
            Me.lblAlmacen.Text = Me.pcOrigenAlmacen & ":  " & CStr(loRenglones.Rows(0).Item("Nom_Alm")).Trim()
            Me.lblCantidad.Text = goServicios.mObtenerFormatoCadena(Me.pcOrigenCantidad, goServicios.enuOpcionesRedondeo.KN_RedondeoPuntoMedio, Me.pnDecimalesParaCantidad)


            Me.mCargarTablaVacia()

        Else

            Me.grdLotesMediciones.DataBind()

        End If

        Me.grdLotesMediciones.mHabilitarBotonera(llEditable)
        'Me.grdLotesMediciones.plPermitirAgregarRenglon = False
        'Me.grdLotesMediciones.plPermitirEliminarRenglon = False
        Me.grdLotesMediciones.mAlmacenarRenglones()

    End Sub

    Protected Sub grdLotesMediciones_mClicBotonAdicional(lnBoton As vis3Controles.grdListaRenglones.enuBotonesAdicionales) Handles grdLotesMediciones.mClicBotonAdicional

        Select Case lnBoton
            Case vis3Controles.grdListaRenglones.enuBotonesAdicionales.lnPrimerBoton


                Dim loTabla As DataTable = Me.grdLotesMediciones.poOrigenDeDatos
                'Dim loPLantilla As DataRow = Me.poArticuloPlantilla.Rows(0)

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

    Private Sub mInicializar()

    End Sub

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

            'Si no indica el artículo: el renglón no se guarda
            If (lcLote = "") Then
                laVacios.Add(lnRenglon)
                Continue For
            End If

            'If (lcLote <> "" And lnCantidad = 0D) Then
            '    If loMensaje.Length = 0 Then
            '        loMensaje.Append("Los siguientes datos no son válidos: <br/>")
            '    End If
            '    loMensaje.Append("* Renglón ")
            '    loMensaje.Append(CInt(loRenglon("Renglon")))
            '    loMensaje.Append(": ")

            '    loMensaje.Append("El lote/colada ")
            '    loMensaje.Append(CStr(loRenglon("cod_lot")).Trim())
            '    loMensaje.Append(" no tiene especificada una cantidad. ")
            'End If

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

        'Elimina los renglones Vacíos (sin nombre de artículo)
        'Desde el último al primero
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

        Dim lcDocumento As String = goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento)
        Dim lcRenglonOrigen As String = goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon)


        loConsulta.AppendLine("DECLARE @lcCod_Art AS VARCHAR(8) = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenArticulo))
        loConsulta.AppendLine("DECLARE @lcCod_Alm AS VARCHAR(10) = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenAlmacen))
        loConsulta.AppendLine("DECLARE @lcTabla AS VARCHAR(30) = " & goServicios.mObtenerCampoFormatoSQL(Me.pcTablaDocumento))
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lcLote AS VARCHAR(30) ")
        loConsulta.AppendLine("DECLARE @lnCan_Lote AS DECIMAL(28,2)")
        loConsulta.AppendLine("DECLARE @lnCan_Piezas AS DECIMAL(28,2)")
        loConsulta.AppendLine("DECLARE @lnPorc_Desp AS DECIMAL(28,2)")
        loConsulta.AppendLine("DECLARE @lnLongitud AS DECIMAL(28,2) ")
        loConsulta.AppendLine("DECLARE @RC INT")
        loConsulta.AppendLine("DECLARE @lcProximoContador VARCHAR(10)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lcUsuario CHAR(10) = " & goServicios.mObtenerCampoFormatoSQL(goUsuario.pcCodigo) & ";")
        loConsulta.AppendLine("DECLARE @lcEquipo CHAR(30) = " & goServicios.mObtenerCampoFormatoSQL(goAuditoria.pcNombreEquipo) & ";")
        loConsulta.AppendLine("DECLARE @lcSucursal CHAR(10) = " & goServicios.mObtenerCampoFormatoSQL(goSucursal.pcCodigo) & ";")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("CREATE TABLE #tmpLoteMediciones (Renglon INT,")
        loConsulta.AppendLine("								 Contador VARCHAR(10),")
        loConsulta.AppendLine("								 Lote VARCHAR(30),")
        loConsulta.AppendLine("								 Cantidad_Lote DECIMAL(28,2),")
        loConsulta.AppendLine("								 Piezas DECIMAL(28,2),")
        loConsulta.AppendLine("								 Porc_Desperdicio DECIMAL(28,2),")
        loConsulta.AppendLine("								 Longitud DECIMAL (28,2))")
        loConsulta.AppendLine("")

        For Each loRenglon As DataRow In loRenglones.Rows

            Dim lnRenglon As Integer = CInt(loRenglon("Renglon"))

            loConsulta.Append("INSERT INTO #tmpLoteMediciones VALUES (")
            loConsulta.Append(lnRenglon.ToString()) 'Renglon
            loConsulta.Append(", ")
            loConsulta.Append("0")                  'Contador
            loConsulta.Append(", ")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("cod_lot")))) 'lote
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("can_lot")))) 'cantidad lote
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("can_pza")))) 'piezas
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("prc_des")))) 'porcentaje desperdicio
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("med_lng")))) 'longitud
            loConsulta.AppendLine(");")

        Next loRenglon

        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lnFilas AS INT = (SELECT MAX(Renglon) FROM #tmpLoteMediciones)")
        loConsulta.AppendLine("DECLARE @lnRenglon AS INT = 1")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("WHILE (@lnRenglon <= @lnFilas)")
        loConsulta.AppendLine("BEGIN ")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	SET @lcLote = (SELECT Lote FROM #tmpLoteMediciones WHERE Renglon = @lnRenglon)")
        loConsulta.AppendLine("	SET @lnCan_Lote = (SELECT Cantidad_Lote FROM #tmpLoteMediciones WHERE Renglon = @lnRenglon)")
        loConsulta.AppendLine("	SET @lnCan_Piezas = (SELECT Piezas FROM #tmpLoteMediciones WHERE Renglon = @lnRenglon)")
        loConsulta.AppendLine("	SET @lnPorc_Desp = (SELECT Porc_Desperdicio FROM #tmpLoteMediciones WHERE Renglon = @lnRenglon)")
        loConsulta.AppendLine("	SET @lnLongitud = (SELECT Longitud FROM #tmpLoteMediciones WHERE Renglon = @lnRenglon)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	IF NOT EXISTS (SELECT 1 FROM Lotes WHERE Cod_Lot = @lcLote AND Cod_Art = @lcCod_Art)")
        loConsulta.AppendLine("	BEGIN ")
        loConsulta.AppendLine("		INSERT INTO Lotes (Cod_Art, Cod_Lot, Exi_Act1) VALUES (@lcCod_Art, @lcLote,@lnCan_Lote)")
        loConsulta.AppendLine("		INSERT INTO Renglones_Lotes (Cod_Alm, Cod_Art, Cod_Lot, Exi_Act1) VALUES (@lcCod_Alm, @lcCod_Art, @lcLote, @lnCan_Lote)")
        loConsulta.AppendLine("	END ")
        loConsulta.AppendLine("	ELSE")
        loConsulta.AppendLine("	BEGIN ")
        loConsulta.AppendLine("		UPDATE Lotes SET Exi_Act1 = Exi_Act1 + @lnCan_Lote WHERE Cod_Lot = @lcLote AND Cod_Art = @lcCod_Art")
        loConsulta.AppendLine("		UPDATE Renglones_Lotes SET Exi_Act1 = Exi_Act1 + @lnCan_Lote")
        loConsulta.AppendLine("		WHERE Cod_Lot = @lcLote AND Cod_Alm = @lcCod_Alm AND Cod_Art = @lcCod_Art")
        loConsulta.AppendLine("	END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine(" IF @lnCan_Lote <> 0 ")
        loConsulta.AppendLine("     BEGIN")
        loConsulta.AppendLine("	        INSERT INTO Operaciones_Lotes (Cod_Alm, Cod_Art, Cod_Lot, Cantidad, Num_Doc, Renglon, Tip_Doc, Tip_Ope, Ren_Ori)")
        loConsulta.AppendLine("	        VALUES (@lcCod_Alm, @lcCod_Art, @lcLote, @lnCan_Lote," & lcDocumento & ",@lnRenglon,@lcTabla,'Entrada'," & lcRenglonOrigen & ")")
        loConsulta.AppendLine("     END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	IF @lnCan_Piezas <> 0 OR @lnPorc_Desp <> 0 OR @lnLongitud <> 0")
        loConsulta.AppendLine("	BEGIN")
        loConsulta.AppendLine("		EXECUTE @RC = [dbo].[mObtenerProximoContador] ")
        loConsulta.AppendLine("		'Mediciones'")
        loConsulta.AppendLine("		,@lcSucursal")
        loConsulta.AppendLine("		,'Normal'")
        loConsulta.AppendLine("		,@lcProximoContador OUTPUT")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("		UPDATE #tmpLoteMediciones SET Contador = @lcProximoContador WHERE Renglon = @lnRenglon")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("		INSERT INTO Mediciones (Documento, Adicional, Status, Posicion, ")
        loConsulta.AppendLine("								Cod_Art, Cod_Alm, Cod_Reg, Tip_Med, Num_Lot, Origen, Cod_Suc, Ren_Ori,")
        loConsulta.AppendLine("								Prioridad, Usu_Cre, Usu_Mod, Equ_Cre, Equ_Mod)")
        loConsulta.AppendLine("		VALUES(@lcProximoContador, 'LOTE|'+@lcCod_Art+'|'+@lcCod_Alm+'|'+@lcLote+'|Entrada|'+ " & lcRenglonOrigen & ", 'Pendiente', 'Por Iniciar Medicion', ")
        loConsulta.AppendLine("				@lcCod_Art, @lcCod_Alm," & lcDocumento & ", 'Prueba',@lcLote, @lcTabla, @lcSucursal, (SELECT TOP 1 CAST(Renglon AS CHAR) FROM #tmpLoteMediciones WHERE Lote = @lcLote ORDER BY Renglon ASC),")
        loConsulta.AppendLine("				'Media', @lcUsuario, @lcUsuario, @lcEquipo, @lcEquipo)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("		IF RTRIM(@lcTabla) = 'Recepciones'")
        loConsulta.AppendLine("		BEGIN")
        loConsulta.AppendLine("			INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("			VALUES (@lcProximoContador, 1, 'NREC-NPIEZ', 'NÚMERO DE PIEZAS NOTAS DE RECEPCIÓN','UND', 1, 99999, @lnCan_Piezas, ")
        loConsulta.AppendLine("					CASE WHEN @lnCan_Piezas = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("			INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("			VALUES (@lcProximoContador, 2, 'NREC-PDESP', 'PORCENTAJE DE DESPERDICIO NOTAS DE RECEPCIÓN','NA', 0, 99, @lnPorc_Desp, ")
        loConsulta.AppendLine("					CASE WHEN @lnPorc_Desp = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("		END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("		IF RTRIM(@lcTabla) = 'Traslados'")
        loConsulta.AppendLine("		BEGIN")
        loConsulta.AppendLine("			INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("			VALUES (@lcProximoContador, 1, 'TA-NPIEZ', 'NÚMERO DE PIEZAS TRASLADOS','UND', 1, 99999, @lnCan_Piezas, ")
        loConsulta.AppendLine("					CASE WHEN @lnCan_Piezas = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("			INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("			VALUES (@lcProximoContador, 2, 'TA-PDESP', 'PORCENTAJE DE DESPERDICIO TRASLADOS','NA', 0, 99, @lnPorc_Desp, ")
        loConsulta.AppendLine("					CASE WHEN @lnPorc_Desp = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("			INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("			VALUES (@lcProximoContador, 3, 'TA-LARG', 'LARGO REAL / TRASLADOS','MTR', 1, 99, @lnLongitud, ")
        loConsulta.AppendLine("					CASE WHEN @lnLongitud = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("		END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("		IF RTRIM(@lcTabla) = 'Ajustes'")
        loConsulta.AppendLine("		BEGIN")
        loConsulta.AppendLine("			INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("			VALUES (@lcProximoContador, 1, 'AINV-NPIEZ', 'NÚMERO DE PIEZAS AJUSTES DE INVENTARIO','UND', 1, 99999, @lnCan_Piezas, ")
        loConsulta.AppendLine("					CASE WHEN @lnCan_Piezas = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("			INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("			VALUES (@lcProximoContador, 2, 'AINV-PDESP', 'PORCENTAJE DE DESPERDICIO AJUSTES DE INVENTARIO','NA', 0, 99, @lnPorc_Desp, ")
        loConsulta.AppendLine("					CASE WHEN @lnPorc_Desp = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("			INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("			VALUES (@lcProximoContador, 3, 'AINV-LARG', 'LARGO REAL / AJUSTES DE INVENTARIO','MTR', 1, 99, @lnLongitud, ")
        loConsulta.AppendLine("					CASE WHEN @lnLongitud = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("		END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	END	")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	SET @lnRenglon = @lnRenglon + 1")
        loConsulta.AppendLine("END ")
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
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")

        Dim lodatos As New goDatos()

        Try
            Dim laSentencias As New ArrayList()
            laSentencias.Add(loConsulta.ToString())

            lodatos.mEjecutarTransaccion(laSentencias)
            Me.mCargarTablaVacia()

            Me.mMostrarMensajeModal("Lotes/Coladas y Mediciones Generados", "Se generó un total de " & loRenglones.Rows.Count & " Lotes. ", "i", False)

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
'-------------------------------------------------------------------------------------------'
' KDE: 01/07/16: Codigo Inicial.								                            '
'-------------------------------------------------------------------------------------------'
' 
