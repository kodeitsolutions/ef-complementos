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
' Inicio de clase "CGS_frmEjecutarAjusteCostos"
'-------------------------------------------------------------------------------------------'
Partial Class CGS_frmEjecutarAjusteCostos
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
#End Region

#Region "Eventos"

    Protected Sub mCargaPagina(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'La primera vez que se cargue el formulario...
        If Not Me.IsPostBack() Then

            Me.txtCan_Dec.pnNumeroDecimales = goOpciones.pnDecimalesParaCantidad
            Me.txtCan_Rev.pnNumeroDecimales = goOpciones.pnDecimalesParaCantidad
            Me.txtAjs_Dif.pnNumeroDecimales = goOpciones.pnDecimalesParaCantidad

            Me.txtCod_Art.mConfigurarBusqueda("Articulos", _
                                              "Cod_Art,Nom_Art", _
                                              "Cod_Art,Nom_Art,Status", _
                                              ".,Código,Nombre,Estatus", _
                                              "Cod_Art,Nom_Art", _
                                              "../../Framework/Formularios/frmFormularioBusqueda.aspx", _
                                              "Cod_Art,Nom_Art", _
                                              "", "")

            'Leer los parámetros enviados al complemento
            Dim laParametros As Generic.Dictionary(Of String, Object)
            laParametros = Me.Session("frmComplementos.paParametros")
            Me.Session.Remove("frmComplementos.paParametros")

            '...si se encontraron los parámetros...
            If (laParametros IsNot Nothing) Then

                'Leer la colección de campos índice del formulario de origen
                Dim laIndices As Generic.Dictionary(Of String, Object)
                laIndices = laParametros("laIndices")
                'Otros parámetros disponibles:
                ' * lcNombreOpcion:	FacturasVentas, OrdenesPagos, Articulos, Clientes...
                ' * lcTabla:        tabla asociada a lcNombreOpcion (facturas, ordenes_compras)
                ' * laIndices:      Diccionario con los nombres y valores de los campos índice 
                '                   del registro seleccionado al abrir el complemento.
                ' * lcCondicion:    cláusula WHERE con la condición para seleccionar el registro 
                '                   indicado por laIndices. (ej: "Documento = '00000236' ")

                'Obtener el o los campos índice usados por el complemento
                'NOTA: Deben ser nombres de campos índice válidos: dependen del formulairo de origen
                Me.pcOrigenDocumento = CStr(laIndices("Documento")).Trim()

                Me.pcTablaDocumento = CStr(laParametros("lcTabla")).Trim()

                Me.txtCan_Rev.Enabled = False

                Me.mCargarDocumento(Me.pcOrigenDocumento)

                End If

            End If

    End Sub

    Protected Sub cmdAceptar_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles cmdAceptar.Click


        Dim lcDocumentoSQL As String = goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento)
        Dim lnDiferencia As Decimal = Me.txtCan_Rev.pbValor - Me.txtCan_Dec.pbValor
        Dim lcArticulo As String = Me.txtCod_Art.pcTexto("Cod_Art")
        'Dim lnTasa As Decimal = goMoneda.mObtenerValorTasaMonedaAdicional()

        Dim loConsulta As New StringBuilder()
        Dim loDatos As New goDatos()

        loConsulta.AppendLine("DECLARE @lcDocumento AS VARCHAR(10) = " & lcDocumentoSQL)
        loConsulta.AppendLine("DECLARE @lcArticulo VARCHAR(8) = " & goServicios.mObtenerCampoFormatoSQL(lcArticulo))
        loConsulta.AppendLine("DECLARE @lnCantidadAju DECIMAL(28,10) = " & goServicios.mObtenerCampoFormatoSQL(lnDiferencia))
        loConsulta.AppendLine("DECLARE @lnCantidadFac DECIMAL(28,10) = " & goServicios.mObtenerCampoFormatoSQL(Me.txtCan_Dec.pbValor))
        loConsulta.AppendLine("DECLARE @RC INT")
        loConsulta.AppendLine("DECLARE @lcProximoContador NVARCHAR(10)")
        loConsulta.AppendLine("DECLARE @lcSucursal NVARCHAR(10)")
        loConsulta.AppendLine("DECLARE @ldFecha DATETIME")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("CREATE TABLE #tmpTemporal (	Renglon		int identity,")
        loConsulta.AppendLine("							Documento	CHAR(100),")
        loConsulta.AppendLine("							Fecha		DATETIME,")
        loConsulta.AppendLine("							Sucursal	CHAR(100),")
        loConsulta.AppendLine("							Articulo	CHAR(300),")
        loConsulta.AppendLine("							Nom_art		CHAR(1000),")
        loConsulta.AppendLine("							Almacen		CHAR(100),")
        loConsulta.AppendLine("							Cantidad	DECIMAL(28,10),")
        loConsulta.AppendLine("							Unidad1		CHAR(100),")
        loConsulta.AppendLine("							Can_uni1	CHAR(130),")
        loConsulta.AppendLine("							Unidad2		CHAR(10),")
        loConsulta.AppendLine("							Can_uni2	CHAR(130),")
        loConsulta.AppendLine("							Precio1		DECIMAL(28,10),")
        loConsulta.AppendLine("							Precio2		DECIMAL(28,10),")
        loConsulta.AppendLine("							Max1		DECIMAL(28,10),")
        loConsulta.AppendLine("							Max2		DECIMAL(28,10))")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("INSERT INTO #tmpTemporal(Documento,Fecha, Articulo,nom_art,almacen,Cantidad,unidad1,can_uni1, unidad2,")
        loConsulta.AppendLine("						can_uni2,Precio1,Precio2,Max1,Max2,Sucursal)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SELECT	Renglones_Compras.documento,")
        loConsulta.AppendLine("		Compras.Fec_Ini,")
        loConsulta.AppendLine("		Renglones_Compras.cod_art,")
        loConsulta.AppendLine("		Articulos.nom_art,")
        loConsulta.AppendLine("		Renglones_Compras.cod_alm,")
        loConsulta.AppendLine("		SUM(Renglones_Compras.Can_Art1) AS Cantidad,")
        loConsulta.AppendLine("		Renglones_Compras.cod_uni,")
        loConsulta.AppendLine("		Renglones_Compras.can_uni,")
        loConsulta.AppendLine("		Renglones_Compras.cod_uni2,")
        loConsulta.AppendLine("		Renglones_Compras.can_uni2,")
        loConsulta.AppendLine("		Renglones_Compras.precio1,")
        loConsulta.AppendLine("		Renglones_Compras.precio2,")
        loConsulta.AppendLine("		MAX(Renglones_Compras.precio1) OVER(Partition by Articulos.cod_art),")
        loConsulta.AppendLine("		MAX(Renglones_Compras.precio2) OVER(Partition by Articulos.cod_art),")
        loConsulta.AppendLine("		Compras.cod_suc")
        loConsulta.AppendLine("FROM Compras")
        loConsulta.AppendLine("	JOIN Renglones_Compras ON Renglones_Compras.Documento = Compras.documento")
        loConsulta.AppendLine("	JOIN Articulos ON Articulos.cod_art = Renglones_Compras.cod_art")
        loConsulta.AppendLine("WHERE Renglones_Compras.documento = @lcDocumento  ")
        loConsulta.AppendLine("      AND Renglones_Compras.cod_art = @lcArticulo")
        loConsulta.AppendLine("GROUP BY Renglones_Compras.documento,Compras.Fec_Ini,Renglones_Compras.cod_art,Articulos.nom_art,Renglones_Compras.cod_alm,")
        loConsulta.AppendLine("		Renglones_Compras.cod_uni,Renglones_Compras.can_uni,Renglones_Compras.cod_uni2,Renglones_Compras.can_uni2,")
        loConsulta.AppendLine("		Renglones_Compras.precio1,Renglones_Compras.precio2,Compras.cod_suc, Articulos.Cod_Art")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SELECT Exi_Act1,Cos_Ult1, Cos_Ult2, Fec_Ult, Cos_Ant1, Cos_Ant2, Cos_Pro1, Cos_Pro2")
        loConsulta.AppendLine("INTO #tmpArticulo")
        loConsulta.AppendLine("FROM Articulos")
        loConsulta.AppendLine("WHERE Cod_Art = @lcArticulo")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lnCostoUlt1 DECIMAL(28,10) ")
        loConsulta.AppendLine("DECLARE @lnCostoPro1 DECIMAL(28,10) ")
        loConsulta.AppendLine("DECLARE @lnCostoUlt2 DECIMAL(28,10) ")
        loConsulta.AppendLine("DECLARE @lnCostoPro2 DECIMAL(28,10)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lnPrecio1 DECIMAL(28,10) = (SELECT Precio1 FROM #tmpTemporal)")
        loConsulta.AppendLine("DECLARE @lnPrecio2 DECIMAL(28,10) = (SELECT Precio2 FROM #tmpTemporal)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SET @lcSucursal = (SELECT TOP 1 Sucursal FROM #tmpTemporal)	")
        loConsulta.AppendLine("SET @ldFecha = (SELECT TOP 1 Fecha FROM #tmpTemporal)	")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lnExi_Art DECIMAL(28,10); ")
        loConsulta.AppendLine("DECLARE @lnCosPro1_Art DECIMAL(28,10);")
        loConsulta.AppendLine("DECLARE @lnCosPro2_Art DECIMAL(28,10);")
        loConsulta.AppendLine("DECLARE @lnCosUlt1_Art DECIMAL(28,10);")
        loConsulta.AppendLine("DECLARE @lnCosUlt2_Art DECIMAL(28,10);")
        loConsulta.AppendLine("DECLARE @lnCosAnt1_Art DECIMAL(28,10);")
        loConsulta.AppendLine("DECLARE @lnCosAnt2_Art DECIMAL(28,10);")
        loConsulta.AppendLine("DECLARE @ldFec_Ult DATETIME;")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SELECT	@lnExi_Art = Exi_Act1,")
        loConsulta.AppendLine("		    @lnCosPro1_Art = Cos_Pro1,")
        loConsulta.AppendLine("		    @lnCosPro2_Art = Cos_Pro2,")
        loConsulta.AppendLine("		    @lnCosUlt1_Art = Cos_Ult1,")
        loConsulta.AppendLine("		    @lnCosUlt2_Art = Cos_Ult2,")
        loConsulta.AppendLine("		    @lnCosAnt1_Art = Cos_Ant1,")
        loConsulta.AppendLine("		    @lnCosAnt2_Art = Cos_Ant2,")
        loConsulta.AppendLine("		    @ldFec_Ult = Fec_Ult")
        loConsulta.AppendLine("FROM #tmpArticulo")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SET @lnCostoUlt1 = ((@lnCantidadFac * @lnPrecio1) + (@lnCantidadAju * 0)) / (@lnCantidadFac + @lnCantidadAju)")
        loConsulta.AppendLine("SET @lnCostoUlt2 = ((@lnCantidadFac * @lnPrecio2) + (@lnCantidadAju * 0)) / (@lnCantidadFac + @lnCantidadAju)")
        loConsulta.AppendLine("SET @lnCostoPro1 = ((@lnCantidadFac * @lnPrecio1) + (@lnCantidadAju * 0) + (@lnExi_Art * @lnCosPro1_Art)) / (@lnCantidadFac + @lnCantidadAju + @lnExi_Art)")
        loConsulta.AppendLine("SET @lnCostoPro2 = ((@lnCantidadFac * @lnPrecio2) + (@lnCantidadAju * 0) + (@lnExi_Art * @lnCosPro2_Art)) / (@lnCantidadFac + @lnCantidadAju + @lnExi_Art)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	EXECUTE @RC = [dbo].[mObtenerProximoContador] ")
        loConsulta.AppendLine("			'AJUINV'")
        loConsulta.AppendLine("			,@lcSucursal")
        loConsulta.AppendLine("			,'Normal'")
        loConsulta.AppendLine("			,@lcProximoContador OUTPUT")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("INSERT INTO Ajustes (Documento,Status,Cod_Mon,Tasa,Numerico1,Tipo,Tip_Ori,Doc_Ori,Comentario,Cod_Suc,Fec_Ini,Fec_Fin)")
        loConsulta.AppendLine("VALUES(@lcProximoContador,'Confirmado','VEB',1.00,@lnCantidadAju,'Costo','Compras',@lcDocumento,")
        loConsulta.AppendLine("			'Ajuste de Inventario generado desde el complemento ''Ajuste de Costos por cantidad revisada'' ',@lcSucursal,@ldFecha,@ldFecha)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("INSERT INTO Renglones_Ajustes (Documento,Cod_Art,Renglon,Cod_Tip,Cod_Alm,Can_Art1,Cos_Ult1,Cos_Ult2,")
        loConsulta.AppendLine("								Mon_Net,Notas,Can_Uni,Cod_Uni,Can_Uni2,Cod_Uni2,Can_Art2,")
        loConsulta.AppendLine("								Ult_Ant1,Ult_Ant2,Cos_Pro1,Cos_Pro2,Cos_Ant1,Cos_Ant2,Tipo)")
        loConsulta.AppendLine("SELECT @lcProximoContador,Articulo,1,'CUMN',almacen,0,@lnCostoUlt1,@lnCostoUlt2,")
        loConsulta.AppendLine("		0,nom_art,can_uni1,unidad1,can_uni2,unidad2,0,")
        loConsulta.AppendLine("		@lnCosUlt1_Art,@lnCosUlt2_Art,@lnCosPro1_Art,@lnCosPro2_Art,@lnCostoUlt1,@lnCostoUlt2,'Cos_Ult1'")
        loConsulta.AppendLine("FROM #tmpTemporal")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("INSERT INTO Renglones_Ajustes (Documento,Cod_Art,Renglon,Cod_Tip,Cod_Alm,Can_Art1,Cos_Ult1,Cos_Ult2,")
        loConsulta.AppendLine("								Mon_Net,Notas,Can_Uni,Cod_Uni,Can_Uni2,Cod_Uni2,Can_Art2,")
        loConsulta.AppendLine("								Ult_Ant1,Ult_Ant2,Cos_Pro1,Cos_Pro2,Cos_Ant1,Cos_Ant2,Tipo)")
        loConsulta.AppendLine("SELECT @lcProximoContador,Articulo,2,'CUOM',almacen,0,@lnCostoUlt1,@lnCostoUlt2,")
        loConsulta.AppendLine("		0,nom_art,can_uni1,unidad1,can_uni2,unidad2,0,")
        loConsulta.AppendLine("		@lnCosUlt1_Art,@lnCosUlt2_Art,@lnCosPro1_Art,@lnCosPro2_Art,@lnCostoUlt1,@lnCostoUlt2,'Cos_Ult2'")
        loConsulta.AppendLine("FROM #tmpTemporal")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("INSERT INTO Renglones_Ajustes (Documento,Cod_Art,Renglon,Cod_Tip,Cod_Alm,Can_Art1,Cos_Ult1,Cos_Ult2,")
        loConsulta.AppendLine("								Mon_Net,Notas,Can_Uni,Cod_Uni,Can_Uni2,Cod_Uni2,Can_Art2,")
        loConsulta.AppendLine("								Ult_Ant1,Ult_Ant2,Cos_Pro1,Cos_Pro2,Cos_Ant1,Cos_Ant2,Tipo)")
        loConsulta.AppendLine("SELECT @lcProximoContador,Articulo,3,'CPMN',almacen,0,@lnCostoPro1,@lnCostoPro2,")
        loConsulta.AppendLine("		0,nom_art,can_uni1,unidad1,can_uni2,unidad2,0,")
        loConsulta.AppendLine("		@lnCosUlt1_Art,@lnCosUlt2_Art,@lnCosPro1_Art,@lnCosPro2_Art,@lnCostoUlt1,@lnCostoUlt2,'Cos_Pro1'")
        loConsulta.AppendLine("FROM #tmpTemporal")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("INSERT INTO Renglones_Ajustes (Documento,Cod_Art,Renglon,Cod_Tip,Cod_Alm,Can_Art1,Cos_Ult1,Cos_Ult2,")
        loConsulta.AppendLine("								Mon_Net,Notas,Can_Uni,Cod_Uni,Can_Uni2,Cod_Uni2,Can_Art2,")
        loConsulta.AppendLine("								Ult_Ant1,Ult_Ant2,Cos_Pro1,Cos_Pro2,Cos_Ant1,Cos_Ant2,Tipo)")
        loConsulta.AppendLine("SELECT @lcProximoContador,Articulo,4,'CPOM',almacen,0,@lnCostoPro1,@lnCostoPro2,")
        loConsulta.AppendLine("		0,nom_art,can_uni1,unidad1,can_uni2,unidad2,0,")
        loConsulta.AppendLine("		@lnCosUlt1_Art,@lnCosUlt2_Art,@lnCosPro1_Art,@lnCosPro2_Art,@lnCostoUlt1,@lnCostoUlt2,'Cos_Pro2'")
        loConsulta.AppendLine("FROM #tmpTemporal")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("UPDATE Articulos ")
        loConsulta.AppendLine("SET Cos_Pro1 = @lnCostoPro1,")
        loConsulta.AppendLine("	Cos_Pro2 = @lnCostoPro2,")
        loConsulta.AppendLine("	Fec_Pro = @ldFecha")
        loConsulta.AppendLine("WHERE Cod_Art = @lcArticulo")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("IF @ldFecha >= @ldFec_Ult")
        loConsulta.AppendLine("BEGIN ")
        loConsulta.AppendLine("	UPDATE Articulos SET Cos_Ant1 = @lnCosUlt1_Art, Cos_Ant2 = @lnCosUlt2_Art, Fec_Ant = @ldFecha")
        loConsulta.AppendLine("	WHERE Cod_Art = @lcArticulo")
        loConsulta.AppendLine("	UPDATE Articulos SET Cos_Ult1 = @lnCostoUlt1, Cos_Ult2 = @lnCostoUlt2, Fec_Ult = @ldFecha ")
        loConsulta.AppendLine("	WHERE Cod_Art = @lcArticulo")
        loConsulta.AppendLine("END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("UPDATE Renglones_Compras SET Caracter2 = 'AJUSTADO' WHERE Documento = @lcDocumento AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lcLote AS VARCHAR(30) = (SELECT Cod_Lot FROM Operaciones_Lotes WHERE Tip_Doc = 'Recepciones' AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("								  AND Num_Doc COLLATE DATABASE_DEFAULT = (SELECT TOP 1 Doc_Ori FROM Renglones_Compras WHERE Documento = @lcDocumento AND Cod_Art = @lcArticulo ORDER BY Renglon ASC)) COLLATE DATABASE_DEFAULT")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("UPDATE Lotes ")
        loConsulta.AppendLine("SET Cos_Pro1 = @lnCostoPro1,	Cos_Pro2 = @lnCostoPro2,")
        loConsulta.AppendLine(" Cos_Ant1 = @lnCosUlt1_Art, Cos_Ant2 = @lnCosUlt2_Art,")
        loConsulta.AppendLine(" Cos_Ult1 = @lnCostoUlt1, Cos_Ult2 = @lnCostoUlt2")
        loConsulta.AppendLine("WHERE Cod_Art = @lcArticulo AND Cod_Lot = @lcLote")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DROP TABLE #tmpTemporal")
        loConsulta.AppendLine("DROP TABLE #tmpArticulo")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")

        Try

            loDatos.mEjecutarTransaccion(New String() {loConsulta.ToString()})

            Dim lcConsulta As String = "SELECT TOP 1 Documento FROM Ajustes ORDER BY Registro DESC;"

            Dim loTabla As DataTable = loDatos.mObtenerTodosSinEsquema(lcConsulta, "Ajustes").Tables(0)

            Dim lcDocumentoAjuste As String = CStr(loTabla.Rows(0).Item("Documento")).Trim()


            'GENERACIÓN DE AUDITORIA:
            Dim laCadenaTransacciones As New ArrayList
            Dim loEjecutarTransaccion As New cusDatos.goDatos

            Dim lcAuditoria As String
            Dim lcTipoAuditoria As String
            Dim lcNombreTabla As String
            Dim lcNombreOpcion As String
            Dim lcAccion As String
            Dim lcDocumento As String
            Dim lcCodigo As String
            Dim lcDetalles As String
            Dim lcNombreEquipo As String
            Dim lcCodigoObjeto As String
            Dim lcNotasRegistro As String
            Dim lcClave2 As String

            lcTipoAuditoria = "'Datos'"
            lcNombreTabla = "'Ajustes'"
            lcNombreOpcion = "'CGS_frmEjecutarAjusteCostos.aspx'"
            lcAccion = "'Agregar'"
            lcDocumento = goServicios.mObtenerCampoFormatoSQL(lcDocumentoAjuste)
            lcCodigo = "'Sin código'"
            lcDetalles = goServicios.mObtenerCampoFormatoSQL(goAuditoria.KC_DetalleVacio)
            lcNombreEquipo = goServicios.mObtenerCampoFormatoSQL(goAuditoria.pcNombreEquipo())
            lcCodigoObjeto = goServicios.mObtenerCampoFormatoSQL(TypeName(Me))
            lcNotasRegistro = "'Documento Agregado desde Complemento ""Ajuste de Costos por cantidad revisada (CGS)""'"
            lcClave2 = "''"

            lcAuditoria = goAuditoria.mObtenerCadenaGuardar(lcTipoAuditoria, lcNombreTabla, lcNombreOpcion, lcAccion, lcDocumento, lcCodigo, lcDetalles, lcNombreEquipo, lcCodigoObjeto, lcNotasRegistro, lcClave2)
            laCadenaTransacciones.Add(lcAuditoria)
            loEjecutarTransaccion.mEjecutarTransaccion(laCadenaTransacciones)

            Me.mMostrarMensajeModal("Ajuste Generado", "Se generó el Ajuste de Inventario #" & lcDocumentoAjuste & ".", "i")

        Catch ex As Exception
            Me.mMostrarMensajeModal("Proceso no Completado", "No fue posible crear el ajuste. Información Adicional: <br/>" & ex.Message, "e")
        End Try

        'goMoneda.pnTasaMonedaAdicional 

    End Sub

    'Protected Sub txtCan_Rev_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtCan_Rev.TextChanged
    Protected Sub txtCan_Rev_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtCan_Rev.TextChanged

        Me.txtAjs_Dif.pbValor = Me.txtCan_Rev.pbValor - Me.txtCan_Dec.pbValor

        If Me.txtAjs_Dif.pbValor = 0D Then
            Me.mMostrarMensajeModal("Operación no válida", "La cantidad declarada y revisada debe ser distinta.", "e")

            Me.cmdAceptar.Enabled = False
            Return
        Else
            Me.cmdAceptar.Enabled = True
        End If

    End Sub

#End Region

#Region "Metodos"

    '    ''' <summary>
    '    ''' Deshabilita los controles del formulario si el cambio de tasa no es 
    '    ''' posible para el documento indicado. 
    '    ''' </summary>
    '    ''' <remarks></remarks>
    Private Sub mDeshabilitarTodo()

        Me.txtCan_Rev.Enabled = False
        Me.cmdAceptar.Visible = False

    End Sub

    '    ''' <summary>
    '    ''' Carga los datos del docmetno indicado y valida si es posible modificar la tasa del mismo. 
    '    ''' </summary>
    '    ''' <param name="lcDocumento"></param>
    '    ''' <param name="lcTabla"></param>
    '    ''' <remarks></remarks>
    Private Sub mCargarDocumento(ByVal lcDocumento As String)

        Dim lcDocumentoSQL As String = goServicios.mObtenerCampoFormatoSQL(lcDocumento)

        Me.txtDocumento.Text = lcDocumento

        Dim loDatosBusqueda As New goDatos()
        Dim loQuery As New StringBuilder()

        loQuery.AppendLine("SELECT Documento, Comentario, Status")
        loQuery.AppendLine("FROM Compras")
        loQuery.AppendLine("WHERE Documento = " & lcDocumentoSQL)
        loQuery.AppendLine("")

        Dim loTabla As DataTable = loDatosBusqueda.mObtenerTodosSinEsquema(loQuery.ToString(), "Recepciones").Tables(0)

        Dim loFilaQuery As DataRow
        loFilaQuery = loTabla.Rows(0)

        If (loTabla Is Nothing) OrElse (loTabla.Rows.Count = 0) Then
            Me.lblTitulo.Text = "Origen desconocido"
            Me.mMostrarMensajeModal("Origen no Válido", "No fue posible obtener la información del documento de origen.", "a")

            Me.mDeshabilitarTodo()
            Return
        End If

        Me.txtComentario.Text = CStr(loFilaQuery("Comentario")).Trim()

        Dim loConsulta As New StringBuilder()

    End Sub

    '    ''' <summary>
    '    ''' Muestra un mensaje modal en pantalla.
    '    ''' </summary>
    '    ''' <param name="lcTitulo"></param>
    '    ''' <param name="lcContenido"></param>
    '    ''' <param name="lcTipo"></param>
    '    ''' <remarks></remarks>
    Private Sub mMostrarMensajeModal(ByVal lcTitulo As String, ByVal lcContenido As String, ByVal lcTipo As String, Optional ByVal llMensajeLargo As Boolean = False)
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

    Protected Sub txtCod_Art_mResultadoBusquedaValido(ByVal sender As vis1Controles.txtNormal, lcNombreCampo As String, lnIndice As Integer) Handles txtCod_Art.mResultadoBusquedaValido
        Dim loTabla As DataTable
        Dim loDatos As New goDatos
        Dim loConsulta As New StringBuilder()

        Dim lcDocumentoSQL As String = goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento)
        Dim lcCod_Art As String = goServicios.mObtenerCampoFormatoSQL(Me.txtCod_Art.pcTexto("Cod_Art"))

        loConsulta.AppendLine("SELECT   Cod_Art,")
        loConsulta.AppendLine("         SUM(Can_Art1) AS Cantidad,")
        loConsulta.AppendLine("         Caracter2")
        loConsulta.AppendLine("FROM Renglones_Compras")
        loConsulta.AppendLine("WHERE Documento = " & lcDocumentoSQL)
        loConsulta.AppendLine(" AND Cod_Art = " & lcCod_Art)
        loConsulta.AppendLine("GROUP BY Cod_Art,Caracter2 ")

        'Me.txtComentario.Text = "ENTRE"

        loTabla = loDatos.mObtenerTodosSinEsquema(loConsulta.ToString(), "Renglones_Compras").Tables(0)

        If (loTabla Is Nothing) OrElse (loTabla.Rows.Count = 0) Then
            Me.lblTitulo.Text = "Origen desconocido"
            Me.mMostrarMensajeModal("Artículo no Válido", "Debe seleccionar un artículo que se encuentre en los renglones de la factura.", "a")

            Me.mDeshabilitarTodo()
            Return
        ElseIf CStr(loTabla.Rows(0).Item("Caracter2")).Trim() = "AJUSTADO" Then
            Me.mMostrarMensajeModal("Artículo no Válido", "Este artículo ya fue ajustado.", "a")

            Me.mDeshabilitarTodo()
            Return
        Else
            Me.txtCan_Dec.pbValor = CDec(loTabla.Rows(0).Item("Cantidad"))
            Me.cmdAceptar.Visible = True
            Me.txtCan_Rev.Enabled = True
        End If

    End Sub

    Protected Sub txtCod_Art_mResultadoBusquedaNoValido(ByVal sender As vis1Controles.txtNormal, lcNombreCampo As String, lnIndice As Integer) Handles txtCod_Art.mResultadoBusquedaNoValido
        Me.mMostrarMensajeModal("Advertencia", _
            "El artículo indicado no es válido.", "a")

        Me.txtCod_Art.pcTexto("Cod_Art") = ""
    End Sub



#End Region


End Class
''-------------------------------------------------------------------------------------------'
'' Fin del codigo																			'
''-------------------------------------------------------------------------------------------'
'' kode it: 14/06/17: Codigo Inicial.								                            '
''-------------------------------------------------------------------------------------------'
