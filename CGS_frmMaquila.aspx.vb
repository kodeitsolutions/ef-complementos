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

        Me.grdConsumido.mRegistrarColumna("cod_art", "Artículo", "", True, True, "String", False, 100)
        Me.grdConsumido.mRegistrarColumna("cod_lot", "Lote", "", True, True, "String", False, 100)
        Me.grdConsumido.mRegistrarColumna("can_art", "Cantidad", 0D, True, True, "Decimal", False, 100)

        Me.grdConsumido.mLimitarCampoTexto("cod_art", True, 50)
        Me.grdConsumido.mLimitarCampoTexto("cod_lot", True, 50)
        Me.grdConsumido.pnDecimalesColumna("can_art") = Me.pnDecimalesParaCantidad

        Me.grdObtenido.mRegistrarColumna("cod_art", "Artículo", "", True, True, "String", False, 100)
        Me.grdObtenido.mRegistrarColumna("cod_lot", "Lote", "", True, True, "String", False, 100)
        Me.grdObtenido.mRegistrarColumna("can_art", "Cantidad", 0D, True, True, "Decimal", False, 100)

        Me.grdObtenido.mLimitarCampoTexto("cod_art", True, 50)
        Me.grdObtenido.mLimitarCampoTexto("cod_lot", True, 50)
        Me.grdObtenido.pnDecimalesColumna("can_art") = Me.pnDecimalesParaCantidad

        Me.grdConsumido.mRegistrarBusquedaAsistida("cod_art", _
                                                    "articulos", _
                                                    "cod_art", _
                                                    "cod_art,nom_art,status", _
                                                    ".,Código,Nombre,E", _
                                                    "cod_art,nom_art", _
                                                    "", "status = \'A\'", False)

        Me.grdConsumido.pcUrlFormularioBusqueda = "../../Framework/Formularios/frmFormularioBusqueda.aspx"

        'Me.grdConsumido.mRegistrarBusquedaAsistida("cod_lot", _
        '                                            "renglones_lotes", _
        '                                            "cod_alm,cod_art,cod_lot", _
        '                                            "cod_lot,exi_act1", _
        '                                            ".,Lote,Disponible", _
        '                                            "cod_lot,exi_act1", _
        '                                            "cod_art:,exi_act1:,can_con:", "exi_act1 > 0", False)

        'Me.grdConsumido.pcUrlFormularioBusqueda = "../../Framework/Formularios/frmFormularioBusqueda.aspx"


        Me.grdObtenido.mRegistrarBusquedaAsistida("cod_art", _
                                                    "articulos", _
                                                    "cod_art", _
                                                    "cod_art,nom_art,status", _
                                                    ".,Código,Nombre,E", _
                                                    "cod_art,nom_art", _
                                                    "", "status = \'A\'", False)

        Me.grdObtenido.pcUrlFormularioBusqueda = "../../Framework/Formularios/frmFormularioBusqueda.aspx"


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
            'Me.txtTotal.pbValor = 0D
            'Me.txtTotal.pnNumeroDecimales = Me.pnDecimalesParaCantidad

        Else
            Me.grdConsumido.DataBind()
            Me.grdObtenido.DataBind()
        End If

        Me.grdConsumido.mHabilitarBotonera(True)
        'Me.grdConsumido.plPermitirAgregarRenglon = False
        'Me.grdConsumido.plPermitirEliminarRenglon = False
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
    ''' Carga la tabla inicial en blanco para el grid de artículos.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub mCargarTablaVacia()

        Dim loTablaConsumido As New DataTable("Consumido")

        loTablaConsumido.Columns.Add(New DataColumn("renglon", GetType(Integer)))
        loTablaConsumido.Columns.Add(New DataColumn("cod_art", GetType(String)))
        loTablaConsumido.Columns.Add(New DataColumn("cod_lot", GetType(String)))
        loTablaConsumido.Columns.Add(New DataColumn("can_art", GetType(Decimal)))

        Dim loTablaObtenido As New DataTable("Obtenido")

        loTablaObtenido.Columns.Add(New DataColumn("renglon", GetType(Integer)))
        loTablaObtenido.Columns.Add(New DataColumn("cod_art", GetType(String)))
        loTablaObtenido.Columns.Add(New DataColumn("cod_lot", GetType(String)))
        loTablaObtenido.Columns.Add(New DataColumn("can_art", GetType(Decimal)))

        For i As Integer = 1 To 1
            Dim loRenglonConsumido As DataRow = loTablaConsumido.NewRow()
            Dim loRenglonObtenido As DataRow = loTablaObtenido.NewRow()

            loRenglonConsumido("Renglon") = i
            loRenglonConsumido("cod_art") = ""
            loRenglonConsumido("cod_lot") = ""
            loRenglonConsumido("can_art") = 0D

            loRenglonObtenido("Renglon") = i
            loRenglonObtenido("cod_art") = ""
            loRenglonObtenido("cod_lot") = ""
            loRenglonObtenido("can_art") = 0D

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
    ''' Valida los datos de los renglones y devuelve true si sin válidos, y false en caso contrario.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function mDatosValidos() As Boolean

        Dim lcAlmConsumo As String = Me.cboAlmacenConsumo.SelectedValue.Trim()
        Dim lcAlmTrabajo As String = Me.cboAlmacenTrabajo.SelectedValue.Trim()

        If lcAlmConsumo = lcAlmTrabajo Then
            Me.mMostrarMensajeModal("Datos no Válidos", "El almacén del artículo obtenido no puede ser igual al del material trabajado.", "a", False)
            Return False
        End If

        Dim laConsumidoVacios As New Generic.List(Of Integer)

        Dim laArtLotes As New Generic.Dictionary(Of String, Object())

        For Each loRenglon As DataRow In grdConsumido.poOrigenDeDatos.Rows

            Dim lnRenglon As Integer = CInt(loRenglon("Renglon"))
            Dim lcArticulo As String = CStr(loRenglon("cod_art"))
            Dim lcLote As String = CStr(loRenglon("cod_lot")).Trim()
            Dim lnCantidad As Decimal = CDec(loRenglon("can_art"))

            If (lcArticulo = "") Then
                laConsumidoVacios.Add(lnRenglon)
                Continue For
            End If

            If Not laArtLotes.ContainsKey(lcArticulo.ToUpper() & "|" & lcLote.ToUpper()) Then
                laArtLotes.Add(lcArticulo.ToUpper() & "|" & lcLote.ToUpper(), New Object() {lnRenglon, lcArticulo, lcLote, lnCantidad})
            Else
                Me.mMostrarMensajeModal("Datos no válidos", "Consumo: Los siguientes datos no son válidos: <br/> Renglón " & lnRenglon & ": " & lcArticulo & "/" & lcLote & " artículo y/o lote repetidos.", "a", False)
            End If

        Next loRenglon

        Dim loConsultaConsumido As New StringBuilder()

        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("CREATE TABLE #tmpValidar(   Cod_Art CHAR(8) COLLATE DATABASE_DEFAULT,")
        loConsultaConsumido.AppendLine("                            Usa_Lot	BIT DEFAULT(0),")
        loConsultaConsumido.AppendLine("							Lote	CHAR(30) COLLATE DATABASE_DEFAULT, ")
        loConsultaConsumido.AppendLine("                            Can_Art DECIMAL(28," & Me.pnDecimalesParaCantidad & "), ")
        loConsultaConsumido.AppendLine("                            Valido  BIT DEFAULT (0),")
        loConsultaConsumido.AppendLine("                            Renglon INT);")
        loConsultaConsumido.AppendLine("")

        For Each lcItem As String In laArtLotes.Keys
            loConsultaConsumido.AppendLine("INSERT INTO #tmpValidar(Cod_Art, Lote, Can_Art, Renglon)")
            loConsultaConsumido.Append("VALUES (" & goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(1)))
            loConsultaConsumido.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(2)))
            loConsultaConsumido.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(3)))
            loConsultaConsumido.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(0)))
            loConsultaConsumido.AppendLine(")")
        Next

        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("DECLARE @lnFilas AS INT = (SELECT MAX(Renglon) FROM #tmpValidar)")
        loConsultaConsumido.AppendLine("DECLARE @lnRenglon AS INT = 1")
        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("WHILE @lnRenglon <= @lnFilas")
        loConsultaConsumido.AppendLine("BEGIN")
        loConsultaConsumido.AppendLine("	UPDATE #tmpValidar ")
        loConsultaConsumido.AppendLine("	SET Usa_Lot = COALESCE((SELECT Usa_Lot FROM Articulos WHERE Cod_Art = (SELECT Cod_Art FROM #tmpValidar WHERE Renglon = @lnRenglon)),0)")
        loConsultaConsumido.AppendLine("	WHERE Renglon = @lnRenglon")
        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("	SET @lnRenglon = @lnRenglon + 1")
        loConsultaConsumido.AppendLine("END")
        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("UPDATE #tmpValidar")
        loConsultaConsumido.AppendLine("SET Valido = 1")
        loConsultaConsumido.AppendLine("FROM Renglones_Lotes  ")
        loConsultaConsumido.AppendLine("WHERE Renglones_Lotes.Cod_Lot = #tmpValidar.Lote ")
        loConsultaConsumido.AppendLine("    AND (Renglones_Lotes.Exi_Act1 > 0 AND #tmpValidar.Can_Art <= Renglones_Lotes.Exi_Act1)")
        loConsultaConsumido.AppendLine("    AND  Renglones_Lotes.Cod_Art = #tmpValidar.Cod_Art")
        loConsultaConsumido.AppendLine("	AND  Renglones_Lotes.Cod_Alm = " & goServicios.mObtenerCampoFormatoSQL(lcAlmConsumo))
        loConsultaConsumido.AppendLine("	AND #tmpValidar.Usa_Lot = 1 ")
        loConsultaConsumido.AppendLine("    AND #tmpValidar.Can_Art > 0")
        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("UPDATE #tmpValidar")
        loConsultaConsumido.AppendLine("SET Valido = 1")
        loConsultaConsumido.AppendLine("FROM Renglones_Almacenes  ")
        loConsultaConsumido.AppendLine("WHERE (Renglones_Almacenes.Exi_Act1 > 0 AND #tmpValidar.Can_Art <= Renglones_Almacenes.Exi_Act1)")
        loConsultaConsumido.AppendLine("    AND  Renglones_Almacenes.Cod_Art = #tmpValidar.Cod_Art")
        loConsultaConsumido.AppendLine("	AND  Renglones_Almacenes.Cod_Alm = " & goServicios.mObtenerCampoFormatoSQL(lcAlmConsumo))
        loConsultaConsumido.AppendLine("	AND #tmpValidar.Usa_Lot = 0 ")
        loConsultaConsumido.AppendLine("	AND #tmpValidar.Lote = ''")
        loConsultaConsumido.AppendLine("    AND #tmpValidar.Can_Art > 0")
        loConsultaConsumido.AppendLine("")
        loConsultaConsumido.AppendLine("SELECT Cod_Art, Usa_Lot, Lote, Can_Art, Renglon ")
        loConsultaConsumido.AppendLine("FROM #tmpValidar")
        loConsultaConsumido.AppendLine("WHERE Valido = 0")
        loConsultaConsumido.AppendLine("ORDER BY Renglon")
        loConsultaConsumido.AppendLine("")

        'Me.TxtComentario.Text = loConsultaConsumido.ToString()
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
                ElseIf CBool(loRenglon("usa_lot")) = False And CStr(loRenglon("lote")).Trim() = "" And CDec(loRenglon("can_art")) > 0D Then
                    loMensaje.Append("* Renglón ")
                    loMensaje.Append(CInt(loRenglon("Renglon")))
                    loMensaje.Append(": ")
                    loMensaje.Append(CStr(loRenglon("cod_art")).Trim())
                    loMensaje.Append(", El artículo no existe o no tiene disponible la cantidad especificada en el almacén de consumo.")
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
                ElseIf CDec(loRenglon("can_art")) = 0D Then
                    loMensaje.Append("* Renglón ")
                    loMensaje.Append(CInt(loRenglon("Renglon")))
                    loMensaje.Append(": ")
                    loMensaje.Append(CStr(loRenglon("cod_art")).Trim())
                    loMensaje.Append(",  La cantidad debe ser mayor a cero (0).")
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

        laConsumidoVacios.Sort()
        laConsumidoVacios.Reverse()

        For Each lnValor As Integer In laConsumidoVacios
            Me.grdConsumido.mEliminarRenglon(lnValor - 1, False, False)
        Next
        'If loMensaje.Length > 0 Then
        '    Me.mMostrarMensajeModal("Datos no Válidos", loMensaje.ToString(), "a", True)
        '    Return False
        'End If

        Dim laObtenidoVacios As New Generic.List(Of Integer)

        laArtLotes.Clear()

        Dim loConsulta As New StringBuilder()

        For Each loRenglon As DataRow In grdObtenido.poOrigenDeDatos.Rows
            Dim lnRenglon As Integer = CInt(loRenglon("Renglon"))
            Dim lcArticulo As String = CStr(loRenglon("cod_art")).Trim()
            Dim lcLote As String = CStr(loRenglon("cod_lot")).Trim()
            Dim lnCantidad As Decimal = CDec(loRenglon("can_art"))

            If (lcArticulo = "") Then
                laObtenidoVacios.Add(lnRenglon)
                Continue For
            End If

            If Not laArtLotes.ContainsKey(lcArticulo.ToUpper() & "|" & lcLote.ToUpper()) Then
                laArtLotes.Add(lcArticulo.ToUpper() & "|" & lcLote.ToUpper(), New Object() {lnRenglon, lcArticulo, lcLote, lnCantidad})
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
        loConsulta.AppendLine("                         Valido  BIT DEFAULT (0),")
        loConsulta.AppendLine("                         Renglon INT);")
        loConsulta.AppendLine("")

        For Each lcItem As String In laArtLotes.Keys
            loConsulta.AppendLine("INSERT INTO #tmpValidar(Cod_Art, Lote, Can_Art, Renglon)")
            loConsulta.Append("VALUES (" & goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(1)))
            loConsulta.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(2)))
            loConsulta.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(3)))
            loConsulta.Append(", ").Append(goServicios.mObtenerCampoFormatoSQL(laArtLotes(lcItem)(0)))
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
        loConsulta.AppendLine("		OR (#tmpValidar.Usa_Lot = 0 AND #tmpValidar.Lote = '')) ")
        loConsulta.AppendLine(" AND #tmpValidar.Can_Art > 0")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SELECT Cod_Art, Usa_Lot, Lote, Can_Art, Renglon, Valido, Repetido")
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
                End If

            Next loRenglon

            Me.mMostrarMensajeModal("Datos no Válidos", loMensaje.ToString(), "a", True)

            Return False
        End If

        'If Me.grdConsumido.mObtenerSumaColumna("can_art") < Me.grdObtenido.mObtenerSumaColumna("can_art") Then
        '    Me.mMostrarMensajeModal("Datos no Válidos", "La cantidad total Obtenida no puede ser mayor que la cantidad Consumida.", "a", True)
        '    Return False
        'End If

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
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lcUsuario AS CHAR(10) = " & goServicios.mObtenerCampoFormatoSQL(goUsuario.pcCodigo))
        loConsulta.AppendLine("DECLARE @lcEmpresa AS CHAR(10) = " & goServicios.mObtenerCampoFormatoSQL(goEmpresa.pcCodigo))
        loConsulta.AppendLine("DECLARE @lcEquipo AS CHAR(30) = " & goServicios.mObtenerCampoFormatoSQL(goAuditoria.pcNombreEquipo()))
        loConsulta.AppendLine("DECLARE @lcSucursal AS CHAR(10) = " & goServicios.mObtenerCampoFormatoSQL(goSucursal.pcCodigo))
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @ldFecha	AS DATETIME = GETDATE()")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("CREATE TABLE #tmpConsumido (Renglon	INT,")
        loConsulta.AppendLine("						       Cod_Art	CHAR(8),")
        loConsulta.AppendLine("							   Lote		CHAR(30),")
        loConsulta.AppendLine("						       Cantidad	DECIMAL(28," & Me.pnDecimalesParaCantidad & "))")
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
            loConsulta.AppendLine(")")

        Next loRenglon

        loConsulta.AppendLine("")
        loConsulta.AppendLine("CREATE TABLE #tmpObtenido (Renglon	INT,")
        loConsulta.AppendLine("							  Cod_Art	CHAR(8),")
        loConsulta.AppendLine("							  Lote		CHAR(30),")
        loConsulta.AppendLine("							  Cantidad	DECIMAL(28," & Me.pnDecimalesParaCantidad & "))")
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
            loConsulta.AppendLine(")")

        Next loRenglon

        loConsulta.AppendLine("")

        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lcMensaje AS VARCHAR(MAX) = ''")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lcArticulo AS VARCHAR(8) = ''")
        loConsulta.AppendLine("DECLARE @lcLote AS VARCHAR(30) = ''")
        loConsulta.AppendLine("DECLARE @lnCantidad AS DECIMAL(28," & Me.pnDecimalesParaCantidad & ") = 0")
        loConsulta.AppendLine("DECLARE @lcUnidad AS VARCHAR(5) = ''")
        loConsulta.AppendLine("DECLARE @lcNom_Art AS VARCHAR(MAX) = ''")
        loConsulta.AppendLine("")
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
        loConsulta.AppendLine("SET @lcMensaje = 'Se generaron los siguientes ajustes de inventario: ' + CHAR(13) + '- CONSUMO: ' + @lcContadorConsumido + '.'")
        loConsulta.AppendLine("				+ CHAR(13) + '- TRABAJADO: ' + @lcContadorObtenido + '.'")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("INSERT INTO Ajustes (Documento,status,automatico,cod_mon,tasa,caracter1,tipo,comentario,cod_suc,fec_ini,fec_fin)")
        loConsulta.AppendLine("VALUES(@lcContadorConsumido,'Confirmado',0,'VEB',1.00,@lcContadorObtenido,'Existencia',")
        loConsulta.AppendLine("	" & goServicios.mObtenerCampoFormatoSQL(Me.TxtComentCons.Text) & ",@lcSucursal,@ldFecha,@ldFecha)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("WHILE (@lnRenglon <= @lnFilas)")
        loConsulta.AppendLine("BEGIN")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	SELECT	@lcArticulo = Cod_Art,")
        loConsulta.AppendLine("			@lcLote = Lote,")
        loConsulta.AppendLine("			@lnCantidad = Cantidad")
        loConsulta.AppendLine("	FROM #tmpConsumido")
        loConsulta.AppendLine("	WHERE Renglon = @lnRenglon")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	SET @lcNom_Art = (SELECT Nom_Art FROM Articulos WHERE Cod_Art = @lcArticulo)")
        loConsulta.AppendLine("	SET @lcUnidad = (SELECT Cod_Uni1 FROM Articulos WHERE Cod_Art = @lcArticulo)	")
        loConsulta.AppendLine("	")
        loConsulta.AppendLine("	INSERT INTO renglones_ajustes (Documento,cod_art,renglon,cod_tip,tipo,cod_alm,can_art1,cos_ult1,")
        loConsulta.AppendLine("								mon_net,notas,can_uni,cod_uni,can_uni2,cod_uni2,can_art2)")
        loConsulta.AppendLine("	VALUES(@lcContadorConsumido,@lcArticulo,@lnRenglon,'S02','Salida',@lcCodAlm_Pro,@lnCantidad,1,")
        loConsulta.AppendLine("		    @lnCantidad, @lcNom_Art,1,@lcUnidad,1,@lcUnidad,@lnCantidad)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	UPDATE Articulos SET Exi_Act1 = Exi_Act1 - @lnCantidad WHERE Cod_Art = @lcArticulo")
        loConsulta.AppendLine("	UPDATE Renglones_Almacenes SET Exi_Act1 = Exi_Act1  - @lnCantidad ")
        loConsulta.AppendLine("	WHERE Cod_ALm = @lcCodAlm_Pro AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("")
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
        loConsulta.AppendLine("	UPDATE Ajustes SET Can_Art1 = (SELECT SUM(Can_Art1) FROM Renglones_Ajustes WHERE Documento = @lcContadorConsumido) WHERE Documento = @lcContadorConsumido")
        loConsulta.AppendLine("	UPDATE Ajustes SET Mon_Net = (SELECT SUM(Mon_Net) FROM Renglones_Ajustes WHERE Documento = @lcContadorConsumido) WHERE Documento = @lcContadorConsumido")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	IF EXISTS(SELECT Documento FROM Ajustes WHERE Documento = @lcContadorConsumido)")
        loConsulta.AppendLine("	BEGIN")
        loConsulta.AppendLine("	    EXECUTE [dbo].[sp_GuardarAuditoria] ")
        loConsulta.AppendLine("            @lcAud_Usuario, @lcAud_Tipo, @lcAud_Tabla, @lcAud_Opcion, @lcAud_Accion,")
        loConsulta.AppendLine("            @lcAud_Documento, @lcAud_Codigo, @lcAud_Clave2, @lcAud_Clave3, @lcAud_Clave4, @lcAud_Clave5,")
        loConsulta.AppendLine("            @lcAud_Detalle, @lcAud_Equipo, @lcAud_Sucursal, @lcAud_Objeto, @lcAud_Notas, @lcAud_Empresa")
        loConsulta.AppendLine("	END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	SET @lnRenglon = @lnRenglon + 1")
        loConsulta.AppendLine("END ")
        loConsulta.AppendLine("")
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
        loConsulta.AppendLine("			@lnCantidad = Cantidad")
        loConsulta.AppendLine("	FROM #tmpObtenido")
        loConsulta.AppendLine("	WHERE Renglon = @lnRenglon")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	SET @lcNom_Art = (SELECT Nom_Art FROM Articulos WHERE Cod_Art = @lcArticulo)")
        loConsulta.AppendLine("	SET @lcUnidad = (SELECT Cod_Uni1 FROM Articulos WHERE Cod_Art = @lcArticulo)	")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	INSERT INTO Renglones_Ajustes (Documento,cod_art,renglon,cod_tip,tipo,cod_alm,can_art1,cos_ult1,")
        loConsulta.AppendLine("								mon_net,notas,can_uni,cod_uni,can_uni2,cod_uni2,can_art2)")
        loConsulta.AppendLine("	VALUES(@lcContadorObtenido,@lcArticulo,@lnRenglon,'E02','Entrada',@lcCodAlm_Tra,@lnCantidad,1,")
        loConsulta.AppendLine("			@lnCantidad, @lcNom_Art,1,@lcUnidad,1,@lcUnidad,@lnCantidad)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	UPDATE Articulos SET Exi_Act1 = Exi_Act1 + @lnCantidad WHERE Cod_Art = @lcArticulo")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	IF NOT EXISTS (SELECT 1 FROM Renglones_Almacenes WHERE Cod_Alm = @lcCodAlm_Tra AND Cod_Art = @lcArticulo)")
        loConsulta.AppendLine("	    INSERT INTO Renglones_Almacenes(Cod_Alm, Cod_Art, Exi_Act1) VALUES (@lcCodAlm_Tra,@lcArticulo, @lnCantidad)")
        loConsulta.AppendLine("	ELSE")
        loConsulta.AppendLine("	    UPDATE Renglones_Almacenes SET Exi_Act1 = Exi_Act1  + @lnCantidad WHERE Cod_ALm = @lcCodAlm_Tra AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("	")
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
        loConsulta.AppendLine("	UPDATE Ajustes SET Can_Art1 = (SELECT SUM(Can_Art1) FROM Renglones_Ajustes WHERE Documento = @lcContadorObtenido) WHERE Documento = @lcContadorObtenido")
        loConsulta.AppendLine("	UPDATE Ajustes SET Mon_Net = (SELECT SUM(Mon_Net) FROM Renglones_Ajustes WHERE Documento = @lcContadorObtenido) WHERE Documento = @lcContadorObtenido")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	IF EXISTS(SELECT Documento FROM Ajustes WHERE Documento = @lcContadorObtenido)")
        loConsulta.AppendLine("	BEGIN")
        loConsulta.AppendLine("	    EXECUTE [dbo].[sp_GuardarAuditoria] ")
        loConsulta.AppendLine("            @lcAud_Usuario, @lcAud_Tipo, @lcAud_Tabla, @lcAud_Opcion, @lcAud_Accion,")
        loConsulta.AppendLine("            @lcAud_Documento, @lcAud_Codigo, @lcAud_Clave2, @lcAud_Clave3, @lcAud_Clave4, @lcAud_Clave5,")
        loConsulta.AppendLine("            @lcAud_Detalle, @lcAud_Equipo, @lcAud_Sucursal, @lcAud_Objeto, @lcAud_Notas, @lcAud_Empresa")
        loConsulta.AppendLine("	END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	SET @lnRenglon = @lnRenglon + 1")
        loConsulta.AppendLine("	")
        loConsulta.AppendLine("END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DROP TABLE #tmpConsumido")
        loConsulta.AppendLine("DROP TABLE #tmpObtenido")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SELECT @lcMensaje AS Mensaje;")
        loConsulta.AppendLine("")

        'Me.TxtComentario.Text = loConsulta.ToString()
        'Return

        Dim lodatos As New goDatos()

        Dim loTabla As DataTable

        Try
            Dim laSentencias As New ArrayList()
            'laSentencias.Add(loConsulta.ToString())

            'lodatos.mEjecutarTransaccion(laSentencias)
            loTabla = lodatos.mObtenerTodosSinEsquema(loConsulta.ToString(), "Mensaje").Tables(0)
            'Me.mMostrarMensajeModal("Ajustes Generados", "Se generaron los siguientes ajustes: </br> Consumos:" & loTabla.Rows(0).Item("Consumo") & " Obtenido: " & loTabla.Rows(0).Item("Trabajo"), "i", False)
            Me.mCargarTablaVacia()
            Me.lblNotificacion.Text = CStr(loTabla.Rows(0).Item("Mensaje")).Trim()
            Me.mMostrarMensajeModal("Ajustes Generados", "Se generaron correctamente los ajustes por consumo y material obtenido.", "i", False)

            Me.cmdAceptar.Enabled = False
            Me.cmdCancelar.Text = "Cerrar"
            Me.grdConsumido.mHabilitarBotonera(False)
            Me.grdObtenido.mHabilitarBotonera(False)

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

    'Private Sub TxtBusqueda_mResultadoBusquedaValido(sender As txtNormal, lcNombreCampo As String, lnIndice As Integer) Handles TxtBusqueda.mResultadoBusquedaValido

    '    Me.mCargarTabla()

    'End Sub

    'Private Sub grdObtenido_mFilaSeleccionada(lnFilaAnterior As Integer, lnFilaNueva As Integer) Handles grdObtenido.mFilaSeleccionada

    'End Sub

    'Private Sub grdConsumido_mFilaSeleccionada(lnFilaAnterior As Integer, lnFilaNueva As Integer) Handles grdConsumido.mFilaSeleccionada

    '    Dim lcArticulo As String = Me.grdConsumido.poValorDatos(Me.grdConsumido.pnIndiceFilaSeleccionada, "cod_art")

    '    Me.TxtComentario.Text = lcArticulo

    '    'Me.grdConsumido.p
    '    If lcArticulo.Trim() = "" Then
    '        Me.mMostrarMensajeModal("Datos no válidos", "Debe primero indicar el artículo", "e", True)
    '    Else
    '        Me.grdConsumido.mDesregistrarBusquedaAsistida("cod_lot")

    '        Me.grdConsumido.mRegistrarBusquedaAsistida("cod_lot", _
    '                                           "renglones_lotes", _
    '                                            "cod_alm,cod_art,cod_lot", _
    '                                            "cod_lot,exi_act1", _
    '                                            ".,Lote,Disponible", _
    '                                            "cod_lot,exi_act1", _
    '                                            "", "cod_art = " & goServicios.mObtenerCampoFormatoSQL(lcArticulo) & " AND cod_alm = " & goServicios.mObtenerCampoFormatoSQL(Me.pcAlmacen) & " AND exi_act1 > 0", False)

    '        Me.grdConsumido.pcUrlFormularioBusqueda = "../../Framework/Formularios/frmFormularioBusqueda.aspx"
    '    End If

    'End Sub



#End Region

#Region " Servicios Web "

#End Region

End Class
'-------------------------------------------------------------------------------------------'
' Fin del codigo																			'
'-------------------------------------------------------------------------------------------'
' KDE: 14/11/17: Codigo Inicial.								                            '
'-------------------------------------------------------------------------------------------'
' 
