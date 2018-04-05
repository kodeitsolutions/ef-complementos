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
Partial Class CGS_frmMaquila
    Inherits vis2formularios.frmFormularioGenerico

#Region "Declaraciones"

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
#End Region

#Region "Eventos"

    Protected Sub mCargaPagina(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.pnDecimalesParaCantidad = goOpciones.pnDecimalesParaCantidad

        'CREACIÓN DE LAS TABLAS
        Me.grdConsumido.mRegistrarColumna("cod_art", "Artículo", "", True, True, "String", False, 100)
        Me.grdConsumido.mRegistrarColumna("cod_lot", "Lote", "", True, True, "String", False, 100)
        Me.grdConsumido.mRegistrarColumna("can_art", "Cantidad", 0D, True, True, "Decimal", False, 100)
        Me.grdConsumido.mRegistrarColumna("can_pza", "Piezas", 0D, True, True, "Decimal", False, 100)
        Me.grdConsumido.mRegistrarColumna("prc_des", "Porcentaje de Desperdicio", 0D, True, True, "Decimal", False, 100)

        Me.grdConsumido.mLimitarCampoTexto("cod_art", True, 50)
        Me.grdConsumido.mLimitarCampoTexto("cod_lot", True, 50)
        Me.grdConsumido.pnDecimalesColumna("can_art") = Me.pnDecimalesParaCantidad
        Me.grdConsumido.pnDecimalesColumna("can_pza") = Me.pnDecimalesParaCantidad
        Me.grdConsumido.pnDecimalesColumna("prc_des") = Me.pnDecimalesParaCantidad

        Me.grdObtenido.mRegistrarColumna("cod_art", "Artículo", "", True, True, "String", False, 100)
        Me.grdObtenido.mRegistrarColumna("cod_lot", "Lote", "", True, True, "String", False, 100)
        Me.grdObtenido.mRegistrarColumna("can_art", "Cantidad", 0D, True, True, "Decimal", False, 100)
        Me.grdObtenido.mRegistrarColumna("can_pza", "Piezas", 0D, True, True, "Decimal", False, 100)
        Me.grdObtenido.mRegistrarColumna("prc_des", "Porcentaje de Desperdicio", 0D, True, True, "Decimal", False, 100)

        Me.grdObtenido.mLimitarCampoTexto("cod_art", True, 50)
        Me.grdObtenido.mLimitarCampoTexto("cod_lot", True, 50)
        Me.grdObtenido.pnDecimalesColumna("can_art") = Me.pnDecimalesParaCantidad
        Me.grdObtenido.pnDecimalesColumna("can_pza") = Me.pnDecimalesParaCantidad
        Me.grdObtenido.pnDecimalesColumna("prc_des") = Me.pnDecimalesParaCantidad


        'BÚSQUEDA DE ARTÍCULOS EN LOS RENGLONES DE LAS TABLAS
        Me.grdConsumido.mRegistrarBusquedaAsistida("cod_art", _
                                                    "articulos", _
                                                    "cod_art", _
                                                    "cod_art,nom_art,status", _
                                                    ".,Código,Nombre,E", _
                                                    "cod_art,nom_art", _
                                                    "", "status = \'A\'", False)

        Me.grdConsumido.pcUrlFormularioBusqueda = "../../Framework/Formularios/frmFormularioBusqueda.aspx"

        Me.grdObtenido.mRegistrarBusquedaAsistida("cod_art", _
                                                    "articulos", _
                                                    "cod_art", _
                                                    "cod_art,nom_art,status", _
                                                    ".,Código,Nombre,E", _
                                                    "cod_art,nom_art", _
                                                    "", "status = \'A\'", False)

        Me.grdObtenido.pcUrlFormularioBusqueda = "../../Framework/Formularios/frmFormularioBusqueda.aspx"

        'Me.grdConsumido.mRegistrarBusquedaAsistida("cod_lot", _
        '                                            "renglones_lotes", _
        '                                            "cod_alm,cod_art,cod_lot", _
        '                                            "cod_lot,exi_act1", _
        '                                            ".,Lote,Disponible", _
        '                                            "cod_lot,exi_act1", _
        '                                            "cod_art:,exi_act1:,can_con:", "exi_act1 > 0", False)

        'Me.grdConsumido.pcUrlFormularioBusqueda = "../../Framework/Formularios/frmFormularioBusqueda.aspx"


        Me.txtTotalConsumido.pnNumeroDecimales = Me.pnDecimalesParaCantidad
        Me.txtTotalObtenido.pnNumeroDecimales = Me.pnDecimalesParaCantidad


        'CARGAR COMBOBOXES DE ALMACENES
        If Not Me.IsPostBack() Then

            Me.mCargarTablaVacia()

            Dim loConsulta As New StringBuilder()

            loConsulta.Length = 0

            loConsulta.AppendLine("SELECT RTRIM(Cod_Alm)    AS Cod_Alm")
            loConsulta.AppendLine("FROM Almacenes")
            loConsulta.AppendLine("WHERE Status = 'A' AND Cod_Alm <> 'TRANSITO'")
            loConsulta.AppendLine("ORDER BY Cod_Alm ASC")
            loConsulta.AppendLine("")

            Dim loListaAlmacenes As DataSet

            loListaAlmacenes = (New goDatos()).mObtenerTodosSinEsquema(loConsulta.ToString(), "Almacenes")

            Me.cboAlmacenConsumo.mLlenarLista(loListaAlmacenes)
            Me.cboAlmacenTrabajo.mLlenarLista(loListaAlmacenes)
        Else
            Me.grdConsumido.DataBind()
            Me.grdObtenido.DataBind()
        End If

        Me.grdConsumido.mHabilitarBotonera(True)
        Me.grdConsumido.mAlmacenarRenglones()

        Me.grdObtenido.mHabilitarBotonera(True)
        Me.grdObtenido.mAlmacenarRenglones()

    End Sub

    Protected Sub cmdAceptar_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles cmdAceptar.Click

        If Not Me.mDatosValidos() Then
            Return
        End If
        'Me.txtTotal.pbValor = Me.grdConsumido.mObtenerSumaColumna("can_art")
        Me.mGenerarAjustes(Me.grdConsumido.poOrigenDeDatos, Me.grdObtenido.poOrigenDeDatos)
    End Sub



#End Region

#Region "Metodos"

    ''' <summary>
    ''' Carga la tabla inicial en blanco para el grid de material consumido y obtenido.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub mCargarTablaVacia()
        Me.txtFecha.pdValor = Date.Now()

        Dim loTablaConsumido As New DataTable("Consumido")

        loTablaConsumido.Columns.Add(New DataColumn("renglon", GetType(Integer)))
        loTablaConsumido.Columns.Add(New DataColumn("cod_art", GetType(String)))
        loTablaConsumido.Columns.Add(New DataColumn("cod_lot", GetType(String)))
        loTablaConsumido.Columns.Add(New DataColumn("can_art", GetType(Decimal)))
        loTablaConsumido.Columns.Add(New DataColumn("can_pza", GetType(Decimal)))
        loTablaConsumido.Columns.Add(New DataColumn("prc_des", GetType(Decimal)))

        Dim loTablaObtenido As New DataTable("Obtenido")

        loTablaObtenido.Columns.Add(New DataColumn("renglon", GetType(Integer)))
        loTablaObtenido.Columns.Add(New DataColumn("cod_art", GetType(String)))
        loTablaObtenido.Columns.Add(New DataColumn("cod_lot", GetType(String)))
        loTablaObtenido.Columns.Add(New DataColumn("can_art", GetType(Decimal)))
        loTablaObtenido.Columns.Add(New DataColumn("can_pza", GetType(Decimal)))
        loTablaObtenido.Columns.Add(New DataColumn("prc_des", GetType(Decimal)))

        For i As Integer = 1 To 1
            Dim loRenglonConsumido As DataRow = loTablaConsumido.NewRow()
            Dim loRenglonObtenido As DataRow = loTablaObtenido.NewRow()

            loRenglonConsumido("Renglon") = i
            loRenglonConsumido("cod_art") = ""
            loRenglonConsumido("cod_lot") = ""
            loRenglonConsumido("can_art") = 0D
            loRenglonConsumido("can_pza") = 0D
            loRenglonConsumido("prc_des") = 0D

            loRenglonObtenido("Renglon") = i
            loRenglonObtenido("cod_art") = ""
            loRenglonObtenido("cod_lot") = ""
            loRenglonObtenido("can_art") = 0D
            loRenglonObtenido("can_pza") = 0D
            loRenglonObtenido("prc_des") = 0D

            loTablaConsumido.Rows.Add(loRenglonConsumido)
            loTablaObtenido.Rows.Add(loRenglonObtenido)
        Next

        Me.grdConsumido.poOrigenDeDatos = loTablaConsumido
        Me.grdConsumido.DataBind()
        Me.grdConsumido.mAlmacenarRenglones()

        Me.grdObtenido.poOrigenDeDatos = loTablaObtenido
        Me.grdObtenido.DataBind()
        Me.grdObtenido.mAlmacenarRenglones()

    End Sub

    ''' <summary>
    ''' Valida los datos de los renglones y devuelve true si son válidos, y false en caso contrario.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function mDatosValidos() As Boolean

        Dim lcAlmConsumo As String = Me.cboAlmacenConsumo.SelectedValue.Trim()
        Dim lcAlmTrabajo As String = Me.cboAlmacenTrabajo.SelectedValue.Trim()

        'EL ALMACÉN DE CONSUMO DEBE SER DISTINTO DEL ALMACÉN DE OBTENIDO
        If lcAlmConsumo = lcAlmTrabajo Then
            Me.mMostrarMensajeModal("Datos no Válidos", "El almacén del artículo consumido no puede ser igual al del material trabajado.", "a", False)
            Return False
        End If

        Dim laConsumidoVacios As New Generic.List(Of Integer)

        Dim laArtLotes As New Generic.Dictionary(Of String, Object())

        For Each loRenglon As DataRow In grdConsumido.poOrigenDeDatos.Rows

            Dim lnRenglon As Integer = CInt(loRenglon("Renglon"))
            Dim lcArticulo As String = CStr(loRenglon("cod_art"))
            Dim lcLote As String = CStr(loRenglon("cod_lot")).Trim()
            Dim lnCantidad As Decimal = CDec(loRenglon("can_art"))
            Dim lnPiezas As Decimal = CDec(loRenglon("can_pza"))
            Dim lnPorcDesp As Decimal = CDec(loRenglon("prc_des"))

            If (lcArticulo = "") Then
                laConsumidoVacios.Add(lnRenglon)
                Continue For
            End If

            If Not laArtLotes.ContainsKey(lcArticulo.ToUpper() & "|" & lcLote.ToUpper()) Then
                laArtLotes.Add(lcArticulo.ToUpper() & "|" & lcLote.ToUpper(), New Object() {lnRenglon, lcArticulo, lcLote, lnCantidad, lnPiezas, lnPorcDesp})
            Else
                Me.mMostrarMensajeModal("Datos no válidos", "Consumo: Los siguientes datos no son válidos: <br/> Renglón " & lnRenglon & ": " & lcArticulo & "/" & lcLote & " artículo y/o lote repetidos.", "a", False)
            End If

        Next loRenglon

        Dim loConsultaConsumido As New StringBuilder()

        'VALIDACIÓN DE DATOS EN GRID CONSUMIDO
        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("CREATE TABLE #tmpValidar(   Cod_Art CHAR(8) COLLATE DATABASE_DEFAULT,")
        loConsultaConsumido.AppendLine("                            Usa_Lot	BIT DEFAULT(0),")
        loConsultaConsumido.AppendLine("                            Disponible DECIMAL(28," & Me.pnDecimalesParaCantidad & ") DEFAULT(CAST(0 AS DECIMAL(28," & Me.pnDecimalesParaCantidad & "))), ")
        loConsultaConsumido.AppendLine("							Lote	CHAR(30) COLLATE DATABASE_DEFAULT, ")
        loConsultaConsumido.AppendLine("                            Can_Art DECIMAL(28," & Me.pnDecimalesParaCantidad & "), ")
        loConsultaConsumido.AppendLine("                            Piezas DECIMAL(28," & Me.pnDecimalesParaCantidad & "), ")
        loConsultaConsumido.AppendLine("                            Porc_Desperdicio DECIMAL(28," & Me.pnDecimalesParaCantidad & "), ")
        loConsultaConsumido.AppendLine("                            Valido  BIT DEFAULT (0),")
        loConsultaConsumido.AppendLine("                            Renglon INT);")
        loConsultaConsumido.AppendLine("")

        For Each lcItem As String In laArtLotes.Keys
            loConsultaConsumido.AppendLine("INSERT INTO #tmpValidar(Cod_Art, Lote, Can_Art, Renglon, Piezas, Porc_Desperdicio)")
            loConsultaConsumido.Append("VALUES (" & goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(1)))
            loConsultaConsumido.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(2)))
            loConsultaConsumido.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(3)))
            loConsultaConsumido.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(0)))
            loConsultaConsumido.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(4)))
            loConsultaConsumido.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(5)))
            loConsultaConsumido.AppendLine(")")
        Next

        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("DECLARE @lnFilas AS INT = (SELECT MAX(Renglon) FROM #tmpValidar)")
        loConsultaConsumido.AppendLine("DECLARE @lnRenglon AS INT = 1")
        loConsultaConsumido.AppendLine("DECLARE @lcArticulo AS VARCHAR(8) = ''")
        loConsultaConsumido.AppendLine("DECLARE @lcLote AS VARCHAR(30) = ''")
        loConsultaConsumido.AppendLine("DECLARE @lcAlmacen AS VARCHAR(15) = " & goServicios.mObtenerCampoFormatoSQL(lcAlmConsumo))
        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("WHILE @lnRenglon <= @lnFilas")
        loConsultaConsumido.AppendLine("BEGIN")
        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("	SELECT @lcArticulo = Cod_Art,")
        loConsultaConsumido.AppendLine("			@lcLote = Lote")
        loConsultaConsumido.AppendLine("	FROM #tmpValidar")
        loConsultaConsumido.AppendLine("	WHERE Renglon = @lnRenglon")
        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("	UPDATE #tmpValidar ")
        loConsultaConsumido.AppendLine("	SET Usa_Lot = COALESCE((SELECT Usa_Lot FROM Articulos WHERE Cod_Art = (SELECT Cod_Art FROM #tmpValidar WHERE Renglon = @lnRenglon)),0)")
        loConsultaConsumido.AppendLine("	WHERE Renglon = @lnRenglon")
        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("	UPDATE #tmpValidar ")
        loConsultaConsumido.AppendLine("	SET  Disponible = CASE WHEN (SELECT Usa_Lot FROM #tmpValidar WHERE Renglon = @lnRenglon) = 1 ")
        loConsultaConsumido.AppendLine("						   THEN COALESCE((SELECT Exi_Act1 FROM Renglones_Lotes ")
        loConsultaConsumido.AppendLine("										  WHERE Cod_Art = @lcArticulo AND Cod_Lot = @lcLote AND Cod_Alm = @lcAlmacen ),0)")
        loConsultaConsumido.AppendLine("						   ELSE COALESCE((SELECT Exi_Act1 FROM Renglones_Almacenes ")
        loConsultaConsumido.AppendLine("										   WHERE Cod_Art = @lcArticulo AND Cod_Alm = @lcAlmacen ),0) ")
        loConsultaConsumido.AppendLine("					  END")
        loConsultaConsumido.AppendLine("	WHERE Renglon = @lnRenglon")
        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("	SET @lnRenglon = @lnRenglon + 1")
        loConsultaConsumido.AppendLine("END")
        loConsultaConsumido.AppendLine("")
        'SI EL ARTÍCULO USA LOTE, SE VERIFICA QUE EL LOTE EXISTA Y TENGA DISPONIBLE LA CANTIDAD ESPECIFICADA EN EL ALMACÉN INDICADO
        'TAMBIÉN SE VERIFICA QUE LA CANTIDAD SEA MAYOR A CERO(0)
        loConsultaConsumido.AppendLine("UPDATE #tmpValidar")
        loConsultaConsumido.AppendLine("SET Valido = 1,")
        loConsultaConsumido.AppendLine("    Disponible = Renglones_Lotes.Exi_Act1")
        loConsultaConsumido.AppendLine("FROM Renglones_Lotes  ")
        loConsultaConsumido.AppendLine("WHERE Renglones_Lotes.Cod_Lot = #tmpValidar.Lote ")
        loConsultaConsumido.AppendLine("    AND (Renglones_Lotes.Exi_Act1 > 0 AND #tmpValidar.Can_Art <= Renglones_Lotes.Exi_Act1)")
        loConsultaConsumido.AppendLine("    AND  Renglones_Lotes.Cod_Art = #tmpValidar.Cod_Art")
        loConsultaConsumido.AppendLine("	AND  Renglones_Lotes.Cod_Alm = @lcAlmacen")
        loConsultaConsumido.AppendLine("	AND #tmpValidar.Usa_Lot = 1 ")
        loConsultaConsumido.AppendLine("    AND #tmpValidar.Can_Art > 0")
        loConsultaConsumido.AppendLine("")
        'SI EL ARTÍCULO NO USA LOTE, SE VERIFICA QUE EL ARTÍCULO EXISTA Y TENGA DISPONIBLE LA CANTIDAD ESPECIFICADA EN EL ALMACÉN INDICADO
        'TAMBIÉN SE VERIFICA QUE LA CANTIDAD SEA MAYOR A CERO(0) 
        loConsultaConsumido.AppendLine("UPDATE #tmpValidar")
        loConsultaConsumido.AppendLine("SET Valido = 1,")
        loConsultaConsumido.AppendLine("    Disponible = Renglones_Almacenes.Exi_Act1")
        loConsultaConsumido.AppendLine("FROM Renglones_Almacenes  ")
        loConsultaConsumido.AppendLine("WHERE (Renglones_Almacenes.Exi_Act1 > 0 AND #tmpValidar.Can_Art <= Renglones_Almacenes.Exi_Act1)")
        loConsultaConsumido.AppendLine("    AND  Renglones_Almacenes.Cod_Art = #tmpValidar.Cod_Art")
        loConsultaConsumido.AppendLine("	AND  Renglones_Almacenes.Cod_Alm = @lcAlmacen")
        loConsultaConsumido.AppendLine("	AND #tmpValidar.Usa_Lot = 0 ")
        loConsultaConsumido.AppendLine("	AND #tmpValidar.Lote = ''")
        loConsultaConsumido.AppendLine("    AND #tmpValidar.Can_Art > 0")
        loConsultaConsumido.AppendLine("    AND #tmpValidar.Piezas = 0 ")
        loConsultaConsumido.AppendLine("    AND #tmpValidar.Porc_Desperdicio = 0")
        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("SELECT Cod_Art, Usa_Lot, Disponible, Lote, Can_Art, Renglon, Piezas, Porc_Desperdicio ")
        loConsultaConsumido.AppendLine("FROM #tmpValidar")
        loConsultaConsumido.AppendLine("WHERE Valido = 0")
        loConsultaConsumido.AppendLine("ORDER BY Renglon")
        loConsultaConsumido.AppendLine("")

        'Me.TxtComentCons.Text = loConsultaConsumido.ToString()
        'Return False

        Dim loConsumido As DataTable

        Try
            loConsumido = (New goDatos()).mObtenerTodosSinEsquema(loConsultaConsumido.ToString(), "ValidacionConsumo").Tables(0)
        Catch ex As Exception
            Me.mMostrarMensajeModal("Operacion no Completada", _
                                    "No fue posible validar los datos ingresados. Información Adicional:<br/>" _
                                    & ex.Message, "e", True)
            Return False
        End Try

        Dim loMensaje As New StringBuilder()

        If (loConsumido.Rows.Count > 0) Then

            loMensaje.Append("Consumo: Los siguientes datos no son válidos: <br/>")

            For Each loRenglon As DataRow In loConsumido.Rows
                If CBool(loRenglon("usa_lot")) = False And CStr(loRenglon("lote")).Trim() <> "" Then
                    loMensaje.Append("* Renglón ")
                    loMensaje.Append(CInt(loRenglon("Renglon")))
                    loMensaje.Append(": ")
                    loMensaje.Append(CStr(loRenglon("cod_art")).Trim())
                    loMensaje.Append(", El artículo no maneja lote y se colocó un lote.")
                    loMensaje.Append("<br/>")
                ElseIf CBool(loRenglon("usa_lot")) = False And CStr(loRenglon("lote")).Trim() = "" And CDec(loRenglon("can_art")) > 0D And CDec(loRenglon("piezas")) = 0D And CDec(loRenglon("porc_desperdicio")) = 0D Then
                    loMensaje.Append("* Renglón ")
                    loMensaje.Append(CInt(loRenglon("Renglon")))
                    loMensaje.Append(": ")
                    loMensaje.Append(CStr(loRenglon("cod_art")).Trim())
                    loMensaje.Append(", El artículo no existe o no tiene disponible la cantidad especificada en el almacén de consumo.")
                    loMensaje.Append("<br/>")
                    loMensaje.Append("Disponible: " & CDec(loRenglon("disponible")) & ".")
                    loMensaje.Append("<br/>")
                ElseIf CBool(loRenglon("usa_lot")) = True And CStr(loRenglon("lote")).Trim() = "" Then
                    loMensaje.Append("* Renglón ")
                    loMensaje.Append(CInt(loRenglon("Renglon")))
                    loMensaje.Append(": ")
                    loMensaje.Append(CStr(loRenglon("cod_art")).Trim())
                    loMensaje.Append(", El artículo maneja lote y no se colocó un lote.")
                    loMensaje.Append("<br/>")
                ElseIf CBool(loRenglon("usa_lot")) = True And CStr(loRenglon("lote")).Trim() <> "" And CDec(loRenglon("can_art")) > 0D Then
                    loMensaje.Append("* Renglón ")
                    loMensaje.Append(CInt(loRenglon("Renglon")))
                    loMensaje.Append(": ")
                    loMensaje.Append(CStr(loRenglon("cod_art")).Trim())
                    loMensaje.Append(",  El lote no existe o no tiene disponible la cantidad especificada en el almacén de consumo.")
                    loMensaje.Append("<br/>")
                    loMensaje.Append("Disponible: " & CDec(loRenglon("disponible")) & ".")
                    loMensaje.Append("<br/>")
                ElseIf CDec(loRenglon("can_art")) = 0D Then
                    loMensaje.Append("* Renglón ")
                    loMensaje.Append(CInt(loRenglon("Renglon")))
                    loMensaje.Append(": ")
                    loMensaje.Append(CStr(loRenglon("cod_art")).Trim())
                    loMensaje.Append(",  La cantidad debe ser mayor a cero (0).")
                    loMensaje.Append("<br/>")
                ElseIf ((CStr(loRenglon("lote")).Trim() = "" And CDec(loRenglon("piezas")) <> 0D) Or (CStr(loRenglon("lote")).Trim() = "" And CDec(loRenglon("porc_desperdicio")) <> 0D)) Then
                    loMensaje.Append("* Renglón ")
                    loMensaje.Append(CInt(loRenglon("Renglon")))
                    loMensaje.Append(": ")
                    loMensaje.Append(CStr(loRenglon("cod_art")).Trim())
                    loMensaje.Append(",  No puede asignar mediciones si el artículo no maneja lotes.")
                    loMensaje.Append("<br/>")
                End If
            Next loRenglon

            Me.mMostrarMensajeModal("Datos no Válidos", loMensaje.ToString(), "a", True)

            Return False
        End If

        If laConsumidoVacios.Count = Me.grdConsumido.pnTotalFilas Then
            Me.mMostrarMensajeModal("Datos no Válidos", "Debe indicar al menos un artículo consumido.", "a", False)
            Return False
        End If

        'ELIMINAR FILAS VACÍAS DEL GRID CONSUMIDO
        laConsumidoVacios.Sort()
        laConsumidoVacios.Reverse()

        For Each lnValor As Integer In laConsumidoVacios
            Me.grdConsumido.mEliminarRenglon(lnValor - 1, False, False)
        Next
        'FIN VALIDACIÓN GRID CONSUMIDO

        'VALIDACIÓN GRID OBTENIDO
        Dim laObtenidoVacios As New Generic.List(Of Integer)

        laArtLotes.Clear()

        Dim loConsulta As New StringBuilder()

        For Each loRenglon As DataRow In grdObtenido.poOrigenDeDatos.Rows
            Dim lnRenglon As Integer = CInt(loRenglon("Renglon"))
            Dim lcArticulo As String = CStr(loRenglon("cod_art")).Trim()
            Dim lcLote As String = CStr(loRenglon("cod_lot")).Trim()
            Dim lnCantidad As Decimal = CDec(loRenglon("can_art"))
            Dim lnPiezas As Decimal = CDec(loRenglon("can_pza"))
            Dim lnPorcDesp As Decimal = CDec(loRenglon("prc_des"))

            If (lcArticulo = "") Then
                laObtenidoVacios.Add(lnRenglon)
                Continue For
            End If

            If Not laArtLotes.ContainsKey(lcArticulo.ToUpper() & "|" & lcLote.ToUpper()) Then
                laArtLotes.Add(lcArticulo.ToUpper() & "|" & lcLote.ToUpper(), New Object() {lnRenglon, lcArticulo, lcLote, lnCantidad, lnPiezas, lnPorcDesp})
            Else
                Me.mMostrarMensajeModal("Datos no válidos", "Obtenido: Los siguientes datos no son válidos: <br/> Renglón " & lnRenglon & ": " & lcArticulo & "/" & lcLote & " artículo y/o lote repetidos.", "a", False)
                Return False
            End If
        Next loRenglon

        loConsulta.AppendLine("")
        loConsulta.AppendLine("CREATE TABLE #tmpValidar(Cod_Art CHAR(8) COLLATE DATABASE_DEFAULT,")
        loConsulta.AppendLine("                         Usa_Lot	BIT DEFAULT(0),")
        loConsulta.AppendLine("							Lote	CHAR(30) COLLATE DATABASE_DEFAULT, ")
        loConsulta.AppendLine("                         Can_Art DECIMAL(28," & Me.pnDecimalesParaCantidad & "), ")
        loConsulta.AppendLine("                         Piezas DECIMAL(28," & Me.pnDecimalesParaCantidad & "), ")
        loConsulta.AppendLine("                         Porc_Desperdicio DECIMAL(28," & Me.pnDecimalesParaCantidad & "), ")
        loConsulta.AppendLine("                         Valido  BIT DEFAULT (0),")
        loConsulta.AppendLine("                         Renglon INT);")
        loConsulta.AppendLine("")

        For Each lcItem As String In laArtLotes.Keys
            loConsulta.AppendLine("INSERT INTO #tmpValidar(Cod_Art, Lote, Can_Art, Renglon, Piezas, Porc_Desperdicio)")
            loConsulta.Append("VALUES (" & goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(1)))
            loConsulta.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(2)))
            loConsulta.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(3)))
            loConsulta.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(0)))
            loConsulta.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(4)))
            loConsulta.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(5)))
            loConsulta.AppendLine(")")
        Next

        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lnFilas AS INT = (SELECT MAX(Renglon) FROM #tmpValidar)")
        loConsulta.AppendLine("DECLARE @lnRenglon AS INT = 1")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("WHILE @lnRenglon <= @lnFilas")
        loConsulta.AppendLine("BEGIN")
        loConsulta.AppendLine("	UPDATE #tmpValidar ")
        loConsulta.AppendLine("	SET Usa_Lot = COALESCE((SELECT Usa_Lot FROM Articulos WHERE Cod_Art = (SELECT Cod_Art FROM #tmpValidar WHERE Renglon = @lnRenglon)),0)")
        loConsulta.AppendLine("	WHERE Renglon = @lnRenglon")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	SET @lnRenglon = @lnRenglon + 1")
        loConsulta.AppendLine("END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("UPDATE #tmpValidar")
        loConsulta.AppendLine("SET Valido = 1")
        loConsulta.AppendLine("FROM Articulos ")
        loConsulta.AppendLine("WHERE Articulos.Cod_Art = #tmpValidar.Cod_Art")
        loConsulta.AppendLine(" AND Articulos.Status = 'A'")
        loConsulta.AppendLine("	AND Articulos.Usa_Lot = #tmpValidar.Usa_Lot ")
        loConsulta.AppendLine("	AND ((#tmpValidar.Usa_Lot = 1  AND #tmpValidar.Lote <> '')")
        loConsulta.AppendLine("		OR (#tmpValidar.Usa_Lot = 0 AND #tmpValidar.Lote = '' AND #tmpValidar.Porc_Desperdicio <> 0 AND #tmpValidar.Piezas <> 0)) ")
        loConsulta.AppendLine(" AND #tmpValidar.Can_Art > 0")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SELECT Cod_Art, Usa_Lot, Lote, Can_Art, Renglon, Valido, Piezas, Porc_Desperdicio")
        loConsulta.AppendLine("FROM #tmpValidar")
        loConsulta.AppendLine("WHERE Valido = 0")
        loConsulta.AppendLine("ORDER BY Renglon")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DROP TABLE #tmpValidar")
        loConsulta.AppendLine("")

        'Me.TxtComentario.Text = loConsulta.ToString()
        'Return False

        If laObtenidoVacios.Count > 0 Then
            Me.mMostrarMensajeModal("Datos no Válidos", "Hay renglones en Obtenido sin artículos, debe eliminarlos.", "a", False)
            Return False
        End If

        Dim loObtenido As DataTable

        Try
            loObtenido = (New goDatos()).mObtenerTodosSinEsquema(loConsulta.ToString(), "Validacion").Tables(0)
        Catch ex As Exception
            Me.mMostrarMensajeModal("Operacion no Completada", _
                                    "No fue posible validar los datos ingresados. Información Adicional:<br/>" _
                                    & ex.Message, "e", True)
            Return False
        End Try

        If (loObtenido.Rows.Count > 0) Then

            loMensaje.Append("<br/> Los siguientes artículos no son válidos: <br/>")

            For Each loRenglon As DataRow In loObtenido.Rows
                If CBool(loRenglon("usa_lot")) = False And CStr(loRenglon("lote")).Trim() <> "" Then
                    loMensaje.Append("* Renglón ")
                    loMensaje.Append(CInt(loRenglon("Renglon")))
                    loMensaje.Append(": ")
                    loMensaje.Append(CStr(loRenglon("cod_art")).Trim())
                    loMensaje.Append(", El artículo no maneja lote y se colocó un lote.")
                    loMensaje.Append("<br/>")
                ElseIf CBool(loRenglon("usa_lot")) = False And CStr(loRenglon("lote")).Trim() = "" And CDec(loRenglon("can_art")) > 0D Then
                    loMensaje.Append("* Renglón ")
                    loMensaje.Append(CInt(loRenglon("Renglon")))
                    loMensaje.Append(": ")
                    loMensaje.Append(CStr(loRenglon("cod_art")).Trim())
                    loMensaje.Append(", El artículo no existe.")
                    loMensaje.Append("<br/>")
                ElseIf CBool(loRenglon("usa_lot")) = True And CStr(loRenglon("lote")).Trim() = "" And CDec(loRenglon("can_art")) > 0D Then
                    loMensaje.Append("* Renglón ")
                    loMensaje.Append(CInt(loRenglon("Renglon")))
                    loMensaje.Append(": ")
                    loMensaje.Append(CStr(loRenglon("cod_art")).Trim())
                    loMensaje.Append(", El artículo maneja lote y no se colocó un lote.")
                    loMensaje.Append("<br/>")
                ElseIf CDec(loRenglon("can_art")) = 0D Then
                    loMensaje.Append("* Renglón ")
                    loMensaje.Append(CInt(loRenglon("Renglon")))
                    loMensaje.Append(": ")
                    loMensaje.Append(CStr(loRenglon("cod_art")).Trim())
                    loMensaje.Append(",  La cantidad debe ser mayor a cero (0).")
                    loMensaje.Append("<br/>")
                ElseIf ((CStr(loRenglon("lote")).Trim() = "" And CDec(loRenglon("piezas")) <> 0D) Or (CStr(loRenglon("lote")).Trim() = "" And CDec(loRenglon("porc_desperdicio")) <> 0D)) Then
                    loMensaje.Append("* Renglón ")
                    loMensaje.Append(CInt(loRenglon("Renglon")))
                    loMensaje.Append(": ")
                    loMensaje.Append(CStr(loRenglon("cod_art")).Trim())
                    loMensaje.Append(",  No puede asignar mediciones si el artículo no maneja lotes.")
                    loMensaje.Append("<br/>")
                End If

            Next loRenglon

            Me.mMostrarMensajeModal("Datos no Válidos", loMensaje.ToString(), "a", True)

            Return False
        End If

        'NO PERMITE QUE LA CANTIDAD OBTENIDA SEA MAYOR A LA CONSUMIDA
        'If Me.grdConsumido.mObtenerSumaColumna("can_art") < Me.grdObtenido.mObtenerSumaColumna("can_art") Then
        '    Me.mMostrarMensajeModal("Datos no Válidos", "La cantidad total Obtenida no puede ser mayor que la cantidad Consumida.", "a", True)
        '    Return False
        'End If

        'ELIMINAR FILAS VACÍAS DE GRID OBTENIDO
        laObtenidoVacios.Sort()
        laObtenidoVacios.Reverse()

        For Each lnValor As Integer In laObtenidoVacios
            Me.grdObtenido.mEliminarRenglon(lnValor - 1, False, False)
        Next
        'FIN DE VALIDACIÓN GRID OBTENIDO

        'MUESTRA MENSAJE DE ERRORES
        If loMensaje.Length > 0 Then
            Me.mMostrarMensajeModal("Datos no Válidos", loMensaje.ToString(), "a", True)
            Return False
        End If

        Me.grdConsumido.DataBind()

        Me.grdConsumido.mAlmacenarRenglones()

        Return True

    End Function

    Protected Sub mGenerarAjustes(loConsumido As DataTable, loObtenido As DataTable)

        Dim lcAlmacenPro As String = goServicios.mObtenerCampoFormatoSQL(Me.cboAlmacenConsumo.SelectedValue.Trim())
        Dim lcAlmacenTra As String = goServicios.mObtenerCampoFormatoSQL(Me.cboAlmacenTrabajo.SelectedValue.Trim())

        Dim loConsulta As New StringBuilder()

        loConsulta.AppendLine("DECLARE @lcCodAlm_Pro AS VARCHAR(10) = " & lcAlmacenPro)
        loConsulta.AppendLine("DECLARE @lcCodAlm_Tra AS VARCHAR(10) = " & lcAlmacenTra)
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @RC INT")
        loConsulta.AppendLine("DECLARE @lcContadorConsumido VARCHAR(10)")
        loConsulta.AppendLine("DECLARE @lcContadorObtenido VARCHAR(10)")
        loConsulta.AppendLine("DECLARE @lcContadorMediciones VARCHAR(10)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lcUsuario AS CHAR(10) = " & goServicios.mObtenerCampoFormatoSQL(goUsuario.pcCodigo))
        loConsulta.AppendLine("DECLARE @lcEmpresa AS CHAR(10) = " & goServicios.mObtenerCampoFormatoSQL(goEmpresa.pcCodigo))
        loConsulta.AppendLine("DECLARE @lcEquipo AS CHAR(30) = " & goServicios.mObtenerCampoFormatoSQL(goAuditoria.pcNombreEquipo()))
        loConsulta.AppendLine("DECLARE @lcSucursal AS CHAR(10) = " & goServicios.mObtenerCampoFormatoSQL(goSucursal.pcCodigo))
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @ldFecha	AS DATETIME = " & goServicios.mObtenerCampoFormatoSQL(CDate(Me.txtFecha.pdValor)))
        loConsulta.AppendLine("")
        loConsulta.AppendLine("CREATE TABLE #tmpConsumido (Renglon	INT,")
        loConsulta.AppendLine("						       Cod_Art	CHAR(8),")
        loConsulta.AppendLine("							   Lote		CHAR(30),")
        loConsulta.AppendLine("						       Cantidad	DECIMAL(28," & Me.pnDecimalesParaCantidad & "),")
        loConsulta.AppendLine("							   Piezas DECIMAL(28," & Me.pnDecimalesParaCantidad & "),")
        loConsulta.AppendLine("							   Porc_Desperdicio DECIMAL(28," & Me.pnDecimalesParaCantidad & ")")
        loConsulta.AppendLine(")")
        loConsulta.AppendLine("")

        For Each loRenglon As DataRow In loConsumido.Rows 'CONSUMIDO

            Dim lnRenglon As Integer = CInt(loRenglon("Renglon"))

            loConsulta.Append("INSERT INTO #tmpConsumido VALUES (")
            loConsulta.Append(lnRenglon.ToString()) 'RENGLON
            loConsulta.Append(", ")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("cod_art")).ToUpper())) 'ARTÍCULO
            loConsulta.Append(", ")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("cod_lot")))) 'LOTE
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("can_art")))) 'CANTIDAD LOTE
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("can_pza")))) 'PIEZAS
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("prc_des")))) 'PORCENTAJE DE DESPERDICIO
            loConsulta.AppendLine(")")

        Next loRenglon

        loConsulta.AppendLine("")
        loConsulta.AppendLine("CREATE TABLE #tmpObtenido (Renglon	INT,")
        loConsulta.AppendLine("							  Cod_Art	CHAR(8),")
        loConsulta.AppendLine("							  Lote		CHAR(30),")
        loConsulta.AppendLine("							  Cantidad	DECIMAL(28," & Me.pnDecimalesParaCantidad & "),")
        loConsulta.AppendLine("							  Piezas DECIMAL(28," & Me.pnDecimalesParaCantidad & "),")
        loConsulta.AppendLine("							  Porc_Desperdicio DECIMAL(28," & Me.pnDecimalesParaCantidad & ")")
        loConsulta.AppendLine(")")
        loConsulta.AppendLine("")

        For Each loRenglon As DataRow In loObtenido.Rows 'OBTENIDO

            Dim lnRenglon As Integer = CInt(loRenglon("Renglon"))

            loConsulta.Append("INSERT INTO #tmpObtenido VALUES (")
            loConsulta.Append(lnRenglon.ToString()) 'RENGLON
            loConsulta.Append(", ")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("cod_art")).ToUpper())) 'ARTICULO
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("cod_lot")))) 'LOTE
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("can_art")))) 'CANTIDAD
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("can_pza")))) 'PIEZAS
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(CStr(loRenglon("prc_des")))) 'PORCENTAJE DE DESPERDICIO
            loConsulta.AppendLine(")")

        Next loRenglon

        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lcArticulo AS VARCHAR(8) = ''")
        loConsulta.AppendLine("DECLARE @lcLote AS VARCHAR(30) = ''")
        loConsulta.AppendLine("DECLARE @lnCantidad AS DECIMAL(28," & Me.pnDecimalesParaCantidad & ") = 0")
        loConsulta.AppendLine("DECLARE @lnCan_Piezas AS DECIMAL(28," & Me.pnDecimalesParaCantidad & ") = 0")
        loConsulta.AppendLine("DECLARE @lnPorc_Desp AS DECIMAL(28," & Me.pnDecimalesParaCantidad & ") = 0")
        loConsulta.AppendLine("DECLARE @lcUnidad AS VARCHAR(5) = ''")
        loConsulta.AppendLine("DECLARE @lcNom_Art AS VARCHAR(MAX) = ''")
        loConsulta.AppendLine("DECLARE @lnCos_Ult AS DECIMAL(28," & Me.pnDecimalesParaCantidad & ") = 0")
        loConsulta.AppendLine("")
        'DATOS DE AUDITORÍAS
        loConsulta.AppendLine("DECLARE @lcAud_Usuario      NVARCHAR(10) = @lcUsuario")
        loConsulta.AppendLine("DECLARE @lcAud_Tipo         NVARCHAR(15) = 'Datos'")
        loConsulta.AppendLine("DECLARE @lcAud_Tabla        NVARCHAR(30) = 'Ajustes'")
        loConsulta.AppendLine("DECLARE @lcAud_Opcion       NVARCHAR(100) = 'AjustesInventarios'")
        loConsulta.AppendLine("DECLARE @lcAud_Accion       NVARCHAR(10) = 'Agregar'")
        loConsulta.AppendLine("DECLARE @lcAud_Documento    NVARCHAR(10) = ''")
        loConsulta.AppendLine("DECLARE @lcAud_Codigo       NVARCHAR(30) = ''")
        loConsulta.AppendLine("DECLARE @lcAud_Clave2       NVARCHAR(100) = ''")
        loConsulta.AppendLine("DECLARE @lcAud_Clave3       NVARCHAR(100) = ''")
        loConsulta.AppendLine("DECLARE @lcAud_Clave4       NVARCHAR(100) = ''")
        loConsulta.AppendLine("DECLARE @lcAud_Clave5       NVARCHAR(100) = ''")
        loConsulta.AppendLine("DECLARE @lcAud_Detalle      NVARCHAR(MAX) = ''")
        loConsulta.AppendLine("DECLARE @lcAud_Equipo       NVARCHAR(30) = @lcEquipo")
        loConsulta.AppendLine("DECLARE @lcAud_Sucursal     NVARCHAR(10) = @lcSucursal")
        loConsulta.AppendLine("DECLARE @lcAud_Objeto       NVARCHAR(100) = " & goServicios.mObtenerCampoFormatoSQL(TypeName(Me)))
        loConsulta.AppendLine("DECLARE @lcAud_Notas        NVARCHAR(MAX) = 'Documento creado automáticamente desde el complemento Asistente de Maquila.'")
        loConsulta.AppendLine("DECLARE @lcAud_Empresa      NVARCHAR(10) = @lcEmpresa")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lnFilas AS INT   = (SELECT MAX(Renglon) FROM #tmpConsumido)")
        loConsulta.AppendLine("DECLARE @lnRenglon AS INT = 1")
        loConsulta.AppendLine("")
        'CONTADORES PARA LOS AJUSTES
        loConsulta.AppendLine("EXECUTE @RC = [dbo].[mObtenerProximoContador] ")
        loConsulta.AppendLine("	'AJUINV'")
        loConsulta.AppendLine("	,@lcSucursal")
        loConsulta.AppendLine("	,'Normal'")
        loConsulta.AppendLine("	,@lcContadorConsumido OUTPUT")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("EXECUTE @RC = [dbo].[mObtenerProximoContador] ")
        loConsulta.AppendLine("	'AJUINV'")
        loConsulta.AppendLine("	,@lcSucursal")
        loConsulta.AppendLine("	,'Normal'")
        loConsulta.AppendLine("	,@lcContadorObtenido OUTPUT")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SET @lcAud_Documento = @lcContadorConsumido")
        loConsulta.AppendLine("")
        'AJUSTE PARA MATERIAL CONSUMIDO
        loConsulta.AppendLine("INSERT INTO Ajustes (Documento,status,automatico,cod_mon,tasa,caracter1,tipo,comentario,cod_suc,fec_ini,fec_fin)")
        loConsulta.AppendLine("VALUES(@lcContadorConsumido,'Confirmado',0,'VEB',1.00,@lcContadorObtenido,'Existencia',")
        loConsulta.AppendLine("	" & goServicios.mObtenerCampoFormatoSQL(Me.TxtComentCons.Text) & ",@lcSucursal,@ldFecha,@ldFecha)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("WHILE (@lnRenglon <= @lnFilas)")
        loConsulta.AppendLine("BEGIN")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	SELECT	@lcArticulo = Cod_Art,")
        loConsulta.AppendLine("			@lcLote = Lote,")
        loConsulta.AppendLine("			@lnCantidad = Cantidad,")
        loConsulta.AppendLine("			@lnCan_Piezas = Piezas,")
        loConsulta.AppendLine("			@lnPorc_Desp = Porc_Desperdicio")
        loConsulta.AppendLine("	FROM #tmpConsumido")
        loConsulta.AppendLine("	WHERE Renglon = @lnRenglon")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	SET @lcNom_Art = COALESCE((SELECT Nom_Art FROM Articulos WHERE Cod_Art = @lcArticulo),'')")
        loConsulta.AppendLine("	SET @lcUnidad = COALESCE((SELECT Cod_Uni1 FROM Articulos WHERE Cod_Art = @lcArticulo),'')")
        loConsulta.AppendLine("	SET @lnCos_Ult = COALESCE((SELECT Cos_Ult1 FROM Articulos WHERE Cod_Art = @lcArticulo),1)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	INSERT INTO renglones_ajustes (Documento,cod_art,renglon,cod_tip,tipo,cod_alm,can_art1,cos_ult1,")
        loConsulta.AppendLine("								mon_net,notas,can_uni,cod_uni,can_uni2,cod_uni2,can_art2)")
        loConsulta.AppendLine("	VALUES(@lcContadorConsumido,@lcArticulo,@lnRenglon,'S02','Salida',@lcCodAlm_Pro,@lnCantidad,@lnCos_Ult,")
        loConsulta.AppendLine("		    @lnCantidad, @lcNom_Art,1,@lcUnidad,1,@lcUnidad,@lnCantidad)")
        loConsulta.AppendLine("")
        'ACTUALIZAR EXISTENCIAS EN TABLAS articulos Y renglones_almacenes
        loConsulta.AppendLine("	UPDATE Articulos SET Exi_Act1 = Exi_Act1 - @lnCantidad WHERE Cod_Art = @lcArticulo")
        loConsulta.AppendLine("	UPDATE Renglones_Almacenes SET Exi_Act1 = Exi_Act1  - @lnCantidad ")
        loConsulta.AppendLine("	WHERE Cod_ALm = @lcCodAlm_Pro AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("")
        'SI EL ARTÍCULO MANEJA LOTES, ACTUALIZAR  EXISTENCIAS EN TABLAS lotes Y renglones_lotes Y REGISTRAR MOVIMIENTO DEL LOTE EN TABLA operaciones_lotes
        loConsulta.AppendLine("	IF @lcLote <> ''")
        loConsulta.AppendLine("	BEGIN")
        loConsulta.AppendLine("		UPDATE Renglones_Lotes SET Exi_Act1 = Exi_Act1 - @lnCantidad ")
        loConsulta.AppendLine("		WHERE Cod_Lot = @lcLote AND Cod_Alm = @lcCodAlm_Pro AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("		UPDATE Lotes SET Exi_Act1 = Exi_Act1 - @lnCantidad WHERE Cod_Lot = @lcLote AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("		INSERT INTO Operaciones_Lotes (Cod_Alm, Cod_Art, Cod_Lot, Cantidad, Num_Doc, Renglon, Tip_Doc, Tip_Ope, Ren_Ori)")
        loConsulta.AppendLine("		VALUES (@lcCodAlm_Pro, @lcArticulo, @lcLote, ABS(@lnCantidad), @lcContadorConsumido, 1, 'Ajustes_Inventarios',")
        loConsulta.AppendLine("			'Salida',@lnRenglon)")
        loConsulta.AppendLine("	END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	IF @lnCan_Piezas <> 0 OR @lnPorc_Desp <> 0")
        loConsulta.AppendLine("	BEGIN")
        loConsulta.AppendLine("		EXECUTE @RC = [dbo].[mObtenerProximoContador] ")
        loConsulta.AppendLine("		'Mediciones'")
        loConsulta.AppendLine("		,@lcSucursal")
        loConsulta.AppendLine("		,'Normal'")
        loConsulta.AppendLine("		,@lcContadorMediciones OUTPUT")
        loConsulta.AppendLine("")
        'AGREGAR MEDICIONES DE MATERIAL CONSUMIDO
        loConsulta.AppendLine("		INSERT INTO Mediciones (Documento, Adicional, Status, Posicion, ")
        loConsulta.AppendLine("        						Cod_Art, Cod_Alm, Cod_Reg, Tip_Med, Num_Lot, Origen, Cod_Suc, Ren_Ori,")
        loConsulta.AppendLine("        						Prioridad, Usu_Cre, Usu_Mod, Equ_Cre, Equ_Mod)")
        loConsulta.AppendLine("		VALUES(@lcContadorMediciones, 'LOTE|'+@lcArticulo+'|'+@lcCodAlm_Pro+'|'+RTRIM(@lcLote)+'|Salida|'+ CAST(@lnRenglon AS CHAR),")
        loConsulta.AppendLine("			'Pendiente', 'Por Iniciar Medicion', @lcArticulo, @lcCodAlm_Pro,@lcContadorConsumido, 'Prueba',@lcLote, 'Ajustes_Inventarios', @lcSucursal,")
        loConsulta.AppendLine("			@lnRenglon,'Media', @lcUsuario, @lcUsuario, @lcEquipo, @lcEquipo)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("        	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("        	VALUES (@lcContadorMediciones, 1, 'AINV-NPIEZ', 'NÚMERO DE PIEZAS AJUSTES DE INVENTARIO','UND', 1, 99999, @lnCan_Piezas, ")
        loConsulta.AppendLine("        			CASE WHEN @lnCan_Piezas = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("        	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("        	VALUES (@lcContadorMediciones, 2, 'AINV-PDESP', 'PORCENTAJE DE DESPERDICIO AJUSTES DE INVENTARIO','NA', 0, 99, @lnPorc_Desp, ")
        loConsulta.AppendLine("        			CASE WHEN @lnPorc_Desp = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("        	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("        	VALUES (@lcContadorMediciones, 3, 'AINV-LARG', 'LARGO REAL / AJUSTES DE INVENTARIO','MTR', 1, 99, 0,'Pendiente')")
        loConsulta.AppendLine("END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	SET @lnRenglon = @lnRenglon + 1")
        loConsulta.AppendLine("END ")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("UPDATE Ajustes SET Can_Art1 = (SELECT SUM(Can_Art1) FROM Renglones_Ajustes WHERE Documento = @lcContadorConsumido) WHERE Documento = @lcContadorConsumido")
        loConsulta.AppendLine("UPDATE Ajustes SET Mon_Net = (SELECT SUM(Mon_Net) FROM Renglones_Ajustes WHERE Documento = @lcContadorConsumido) WHERE Documento = @lcContadorConsumido")
        loConsulta.AppendLine("")
        'INSERTAR AUDITORÍA SI EXISTE EL DOCUMENTO
        loConsulta.AppendLine("IF EXISTS(SELECT Documento FROM Ajustes WHERE Documento = @lcContadorConsumido)")
        loConsulta.AppendLine("BEGIN")
        loConsulta.AppendLine("    EXECUTE [dbo].[sp_GuardarAuditoria] ")
        loConsulta.AppendLine("           @lcAud_Usuario, @lcAud_Tipo, @lcAud_Tabla, @lcAud_Opcion, @lcAud_Accion,")
        loConsulta.AppendLine("           @lcAud_Documento, @lcAud_Codigo, @lcAud_Clave2, @lcAud_Clave3, @lcAud_Clave4, @lcAud_Clave5,")
        loConsulta.AppendLine("           @lcAud_Detalle, @lcAud_Equipo, @lcAud_Sucursal, @lcAud_Objeto, @lcAud_Notas, @lcAud_Empresa")
        loConsulta.AppendLine("END")
        loConsulta.AppendLine("")
        'AJUSTE PARA MATERIAL OBTENIDO
        loConsulta.AppendLine("SET @lnFilas = (SELECT COUNT(*) FROM #tmpObtenido)")
        loConsulta.AppendLine("SET @lnRenglon = 1")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SET @lcAud_Documento = @lcContadorObtenido")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("INSERT INTO Ajustes (Documento,status,automatico,cod_mon,tasa,caracter1,tipo,comentario,cod_suc,fec_ini,fec_fin)")
        loConsulta.AppendLine("VALUES(@lcContadorObtenido,'Confirmado',0,'VEB',1.00,@lcContadorConsumido,'Existencia',")
        loConsulta.AppendLine("	" & goServicios.mObtenerCampoFormatoSQL(Me.TxtComentObt.Text) & ",@lcSucursal,@ldFecha,@ldFecha)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("WHILE (@lnRenglon <= @lnFilas)")
        loConsulta.AppendLine("BEGIN")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	SELECT	@lcArticulo = Cod_Art,")
        loConsulta.AppendLine("			@lcLote = Lote,")
        loConsulta.AppendLine("			@lnCantidad = Cantidad,")
        loConsulta.AppendLine("			@lnCan_Piezas = Piezas,")
        loConsulta.AppendLine("			@lnPorc_Desp = Porc_Desperdicio")
        loConsulta.AppendLine("	FROM #tmpObtenido")
        loConsulta.AppendLine("	WHERE Renglon = @lnRenglon")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	SET @lcNom_Art = COALESCE((SELECT Nom_Art FROM Articulos WHERE Cod_Art = @lcArticulo),'')")
        loConsulta.AppendLine("	SET @lcUnidad = COALESCE((SELECT Cod_Uni1 FROM Articulos WHERE Cod_Art = @lcArticulo),'')")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	INSERT INTO Renglones_Ajustes (Documento,cod_art,renglon,cod_tip,tipo,cod_alm,can_art1,cos_ult1,")
        loConsulta.AppendLine("								mon_net,notas,can_uni,cod_uni,can_uni2,cod_uni2,can_art2)")
        loConsulta.AppendLine("	VALUES(@lcContadorObtenido,@lcArticulo,@lnRenglon,'E02','Entrada',@lcCodAlm_Tra,@lnCantidad,1,")
        loConsulta.AppendLine("			@lnCantidad, @lcNom_Art,1,@lcUnidad,1,@lcUnidad,@lnCantidad)")
        loConsulta.AppendLine("")
        'ACTUALIZAR EXISTENCIAS EN TABLAS articulos Y renglones_almacenes
        loConsulta.AppendLine("	UPDATE Articulos SET Exi_Act1 = Exi_Act1 + @lnCantidad WHERE Cod_Art = @lcArticulo")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	IF NOT EXISTS (SELECT 1 FROM Renglones_Almacenes WHERE Cod_Alm = @lcCodAlm_Tra AND Cod_Art = @lcArticulo)")
        loConsulta.AppendLine("	    INSERT INTO Renglones_Almacenes(Cod_Alm, Cod_Art, Exi_Act1) VALUES (@lcCodAlm_Tra,@lcArticulo, @lnCantidad)")
        loConsulta.AppendLine("	ELSE")
        loConsulta.AppendLine("	    UPDATE Renglones_Almacenes SET Exi_Act1 = Exi_Act1  + @lnCantidad WHERE Cod_ALm = @lcCodAlm_Tra AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("	")
        'SI EL ARTÍCULO MANEJA LOTES, ACTUALIZAR  EXISTENCIAS EN TABLAS lotes Y renglones_lotes Y REGISTRAR MOVIMIENTO DEL LOTE EN TABLA operaciones_lotes
        loConsulta.AppendLine("	IF @lcLote <> ''")
        loConsulta.AppendLine("	BEGIN")
        loConsulta.AppendLine("		IF NOT EXISTS (SELECT 1 FROM Lotes WHERE Cod_Lot = @lcLote AND Cod_Art = @lcArticulo)")
        loConsulta.AppendLine("		BEGIN ")
        loConsulta.AppendLine("			INSERT INTO Lotes (Cod_Art, Cod_Lot, Exi_Act1) VALUES (@lcArticulo, @lcLote,@lnCantidad)")
        loConsulta.AppendLine("			INSERT INTO Renglones_Lotes (Cod_Alm, Cod_Art, Cod_Lot, Exi_Act1) VALUES (@lcCodAlm_Tra, @lcArticulo, @lcLote, @lnCantidad)")
        loConsulta.AppendLine("		END ")
        loConsulta.AppendLine("		ELSE")
        loConsulta.AppendLine("		BEGIN ")
        loConsulta.AppendLine("			UPDATE Lotes SET Exi_Act1 = Exi_Act1 + @lnCantidad WHERE Cod_Lot = @lcLote AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("			UPDATE Renglones_Lotes SET Exi_Act1 = Exi_Act1 + @lnCantidad")
        loConsulta.AppendLine("			WHERE Cod_Lot = @lcLote AND Cod_Alm = @lcCodAlm_Tra AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("		END")
        loConsulta.AppendLine("	")
        loConsulta.AppendLine("		INSERT INTO Operaciones_Lotes (Cod_Alm, Cod_Art, Cod_Lot, Cantidad, Num_Doc, Renglon, Tip_Doc, Tip_Ope, Ren_Ori)")
        loConsulta.AppendLine("		VALUES (@lcCodAlm_Tra, @lcArticulo, @lcLote, @lnCantidad,@lcContadorObtenido,1,'Ajustes_Inventarios','Entrada',@lnRenglon)")
        loConsulta.AppendLine("	END")
        loConsulta.AppendLine("")
        'AGREGAR MEDICIONES DE MATERIAL OBTENIDO
        loConsulta.AppendLine("	IF @lnCan_Piezas <> 0 OR @lnPorc_Desp <> 0")
        loConsulta.AppendLine("	BEGIN")
        loConsulta.AppendLine("		EXECUTE @RC = [dbo].[mObtenerProximoContador] ")
        loConsulta.AppendLine("		'Mediciones'")
        loConsulta.AppendLine("		,@lcSucursal")
        loConsulta.AppendLine("		,'Normal'")
        loConsulta.AppendLine("		,@lcContadorMediciones OUTPUT")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("		INSERT INTO Mediciones (Documento, Adicional, Status, Posicion, ")
        loConsulta.AppendLine("        						Cod_Art, Cod_Alm, Cod_Reg, Tip_Med, Num_Lot, Origen, Cod_Suc, Ren_Ori,")
        loConsulta.AppendLine("        						Prioridad, Usu_Cre, Usu_Mod, Equ_Cre, Equ_Mod)")
        loConsulta.AppendLine("		VALUES(@lcContadorMediciones, 'LOTE|'+@lcArticulo+'|'+@lcCodAlm_Tra+'|'+RTRIM(@lcLote)+'|Entrada|'+CAST(@lnRenglon AS CHAR),")
        loConsulta.AppendLine("			'Pendiente', 'Por Iniciar Medicion', @lcArticulo, @lcCodAlm_Tra,@lcContadorObtenido, 'Prueba',@lcLote, 'Ajustes_Inventarios', @lcSucursal,")
        loConsulta.AppendLine("			@lnRenglon,'Media', @lcUsuario, @lcUsuario, @lcEquipo, @lcEquipo)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("        	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("        	VALUES (@lcContadorMediciones, 1, 'AINV-NPIEZ', 'NÚMERO DE PIEZAS AJUSTES DE INVENTARIO','UND', 1, 99999, @lnCan_Piezas, ")
        loConsulta.AppendLine("        			CASE WHEN @lnCan_Piezas = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("        	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("        	VALUES (@lcContadorMediciones, 2, 'AINV-PDESP', 'PORCENTAJE DE DESPERDICIO AJUSTES DE INVENTARIO','NA', 0, 99, @lnPorc_Desp, ")
        loConsulta.AppendLine("        			CASE WHEN @lnPorc_Desp = 0 THEN 'Pendiente' ELSE 'Aprobado' END)")
        loConsulta.AppendLine("        	INSERT INTO Renglones_Mediciones (Documento, Renglon, Cod_Var, Nom_Var, Cod_Uni, Val_Min_Esp, Val_Max_Esp, Res_Num, Evaluacion)")
        loConsulta.AppendLine("        	VALUES (@lcContadorMediciones, 3, 'AINV-LARG', 'LARGO REAL / AJUSTES DE INVENTARIO','MTR', 1, 99, 0,'Pendiente')")
        loConsulta.AppendLine("END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	SET @lnRenglon = @lnRenglon + 1")
        loConsulta.AppendLine("	")
        loConsulta.AppendLine("END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("UPDATE Ajustes SET Can_Art1 = (SELECT SUM(Can_Art1) FROM Renglones_Ajustes WHERE Documento = @lcContadorObtenido) WHERE Documento = @lcContadorObtenido")
        loConsulta.AppendLine("UPDATE Ajustes SET Mon_Net = (SELECT SUM(Mon_Net) FROM Renglones_Ajustes WHERE Documento = @lcContadorObtenido) WHERE Documento = @lcContadorObtenido")
        loConsulta.AppendLine("")
        'INSERTAR AUDITORÍA SI EXISTE EL DOCUMENTO
        loConsulta.AppendLine("IF EXISTS(SELECT Documento FROM Ajustes WHERE Documento = @lcContadorObtenido)")
        loConsulta.AppendLine("BEGIN")
        loConsulta.AppendLine("    EXECUTE [dbo].[sp_GuardarAuditoria] ")
        loConsulta.AppendLine("           @lcAud_Usuario, @lcAud_Tipo, @lcAud_Tabla, @lcAud_Opcion, @lcAud_Accion,")
        loConsulta.AppendLine("           @lcAud_Documento, @lcAud_Codigo, @lcAud_Clave2, @lcAud_Clave3, @lcAud_Clave4, @lcAud_Clave5,")
        loConsulta.AppendLine("           @lcAud_Detalle, @lcAud_Equipo, @lcAud_Sucursal, @lcAud_Objeto, @lcAud_Notas, @lcAud_Empresa")
        loConsulta.AppendLine("END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DROP TABLE #tmpConsumido")
        loConsulta.AppendLine("DROP TABLE #tmpObtenido")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SELECT @lcContadorConsumido AS Consumido, @lcContadorObtenido AS Obtenido")
        loConsulta.AppendLine("")

        'Me.TxtComentCons.Text = loConsulta.ToString()
        'Return

        Dim lodatos As New goDatos()

        Dim loTabla As DataTable

        Try
            Dim laSentencias As New ArrayList()
            'laSentencias.Add(loConsulta.ToString())

            'lodatos.mEjecutarTransaccion(laSentencias)
            loTabla = lodatos.mObtenerTodosSinEsquema(loConsulta.ToString(), "Mensaje").Tables(0)

            Me.mCargarTablaVacia()

            Me.mMostrarMensajeModal("Ajustes Generados", "Se generaron correctamente los siguientes ajustes de inventario: <br/> - PROCESADO: " & CStr(loTabla.Rows(0).Item("Consumido")).Trim() & ". <br/> - OBTENIDO: " & CStr(loTabla.Rows(0).Item("Obtenido")).Trim() & ".", "i", False)

            Me.TxtComentCons.Text = ""
            Me.TxtComentObt.Text = ""

            'Me.cmdAceptar.Enabled = False
            Me.cmdCancelar.Text = "Cerrar"
            'Me.grdConsumido.mHabilitarBotonera(False)
            'Me.grdObtenido.mHabilitarBotonera(False)

        Catch ex As Exception

            Dim lcMensaje As String = ex.Message

            Me.mMostrarMensajeModal("Proceso no completado", "No fue posible generar los ajustes. Información Adicional:<br/> " & lcMensaje, "e", True)

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

    Private Sub grdConsumido_mActualizarRenglones(sender As Object, e As EventArgs) Handles grdConsumido.mActualizarRenglones
        Me.txtTotalConsumido.pbValor = Me.grdConsumido.mObtenerSumaColumna("can_art")
    End Sub

    Private Sub grdObtenido_mActualizarRenglones(sender As Object, e As EventArgs) Handles grdObtenido.mActualizarRenglones
        Me.txtTotalObtenido.pbValor = Me.grdObtenido.mObtenerSumaColumna("can_art")
    End Sub

#End Region

#Region " Servicios Web "
    ''' <summary>
    ''' Valida un código de artículo.
    ''' </summary>
    ''' <param name="lcArticulo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Web.Services.WebMethod(True)> _
    Public Shared Function mValidarArticulo(lcArticulo As String) As Generic.Dictionary(Of String, Object)
        Dim loRespuesta As New Generic.Dictionary(Of String, Object)

        Dim lcConsulta As String = "SELECT Cod_Art from Articulos WHERE status = 'A' AND Cod_Art = " & _
            goServicios.mObtenerCampoFormatoSQL(lcArticulo)

        Try

            Dim loDatos As DataTable
            loDatos = (New goDatos()).mObtenerTodosSinEsquema(lcConsulta, "Articulos").Tables(0)

            If (loDatos.Rows.Count > 0) Then

                loRespuesta.Add("llEsValido", True)
                Return loRespuesta

            End If

        Catch ex As Exception

        End Try

        loRespuesta.Add("llEsValido", False)

        Return loRespuesta
    End Function
#End Region

End Class
'-------------------------------------------------------------------------------------------'
' Fin del codigo																			'
'-------------------------------------------------------------------------------------------'
