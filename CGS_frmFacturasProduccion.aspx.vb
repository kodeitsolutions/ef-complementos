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
Partial Class CGS_frmFacturasProduccion
    Inherits vis2formularios.frmFormularioGenerico

#Region "Declaraciones"
    Private Const KN_CANTIDAD_RENGLONES_LOTE As Integer = 0
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
    Private Property pdCantidad() As Decimal
        Get
            Return CStr(Me.ViewState("pdCantidad"))
        End Get
        Set(ByVal value As Decimal)
            Me.ViewState("pdCantidad") = value
        End Set
    End Property
#End Region

#Region "Eventos"

    Protected Sub mCargaPagina(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'La primera vez que se cargue el formulario...
        If Not Me.IsPostBack() Then

            Me.TxtBusqueda.mConfigurarBusqueda("Recepciones", _
                                              "Documento", _
                                              "Documento,Comentario,status", _
                                              ".,Documento,Comentario,Estatus", _
                                              "Documento,Comentario,status", _
                                              "../../Framework/Formularios/frmFormularioBusqueda.aspx", _
                                              "Documento", _
                                              "", "Status = 'Confirmado'")

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

        Me.grdRenglones.mLimitarCampoTexto("cod_art", True, 50)
        Me.grdRenglones.mLimitarCampoTexto("nom_art", True, 50)
        Me.grdRenglones.pnDecimalesColumna("can_art") = Me.pnDecimalesParaCantidad

        If Not Me.IsPostBack() Then

            Dim loConsulta As New StringBuilder()

            loConsulta.AppendLine("SELECT   Renglones_Compras.Cod_Art   AS Cod_Art,")
            loConsulta.AppendLine("         Articulos.Nom_Art           AS Nom_Art,")
            loConsulta.AppendLine("         Renglones_Compras.Cod_Alm   AS Cod_Alm,")
            loConsulta.AppendLine("         Almacenes.Nom_Alm           AS Nom_Alm,")
            loConsulta.AppendLine("         Renglones_Compras.Can_Art1  AS Can_Art,")
            loConsulta.AppendLine("         Renglones_Compras.Doc_Ori   AS Doc_Ori")
            loConsulta.AppendLine("FROM Renglones_Compras")
            loConsulta.AppendLine(" JOIN Articulos ON Articulos.Cod_Art = Renglones_Compras.Cod_Art")
            loConsulta.AppendLine(" JOIN Almacenes ON Almacenes.Cod_Alm = Renglones_Compras.Cod_Alm")
            loConsulta.AppendLine("WHERE Renglones_Compras.Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento))
            loConsulta.AppendLine(" AND Renglones_Compras.Renglon = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon))
            loConsulta.AppendLine("")
            loConsulta.AppendLine("")


            Dim loRenglones As DataTable = (New goDatos()).mObtenerTodosSinEsquema(loConsulta.ToString(), "Renglones_Recepciones").Tables(0)

            'VALIDAR QUE EL RENGLÓN NO TENGA UN ORIGEN ASOCIADO
            If CStr(loRenglones.Rows(0).Item("Doc_Ori")).Trim() <> "" Then
                Me.mMostrarMensajeModal("Operación no permitida", "Este renglón ya tiene un origen asociado.", "a")
                Me.cmdAceptar.Enabled = False
                Return
            End If

            Me.lblArticulo.Text = CStr(loRenglones.Rows(0).Item("Cod_Art")).Trim() & ":  " & CStr(loRenglones.Rows(0).Item("Nom_Art")).Trim()
            Me.lblAlmacen.Text = CStr(loRenglones.Rows(0).Item("Cod_Alm")).Trim() & ":  " & CStr(loRenglones.Rows(0).Item("Nom_Alm")).Trim()
            Me.lblRenglon.Text = " " & Me.pcOrigenRenglon
            Me.lblCantidad.Text = goServicios.mObtenerFormatoCadena(CDec(loRenglones.Rows(0).Item("Can_Art")), goServicios.enuOpcionesRedondeo.KN_RedondeoPuntoMedio, Me.pnDecimalesParaCantidad)

            Me.pdCantidad = CDec(loRenglones.Rows(0).Item("Can_Art"))

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

            Dim lcDocumentoSQL As String = goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento)
            Dim lnDiferencia As Decimal = Me.grdRenglones.poValorDatos(Me.grdRenglones.pnIndiceFilaSeleccionada, "Can_Art") - Me.pdCantidad
            Dim lcArticulo As String = Me.grdRenglones.poValorDatos(Me.grdRenglones.pnIndiceFilaSeleccionada, "Cod_Art")

            Dim loConsulta As New StringBuilder()
            Dim loDatos As New goDatos()

            loConsulta.AppendLine("DECLARE @lcDocumento AS VARCHAR(10) = " & lcDocumentoSQL)
            loConsulta.AppendLine("DECLARE @lcArticulo VARCHAR(8) = " & goServicios.mObtenerCampoFormatoSQL(lcArticulo))
            loConsulta.AppendLine("DECLARE @lnRngFact INT = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon))
            loConsulta.AppendLine("DECLARE @lnCantidadAju DECIMAL(28,10) = " & goServicios.mObtenerCampoFormatoSQL(lnDiferencia))
            loConsulta.AppendLine("DECLARE @lnCantidadFac DECIMAL(28,10) = " & goServicios.mObtenerCampoFormatoSQL(Me.pdCantidad))
            loConsulta.AppendLine("DECLARE @RC INT")
            loConsulta.AppendLine("DECLARE @lcProximoContador NVARCHAR(10)")
            loConsulta.AppendLine("DECLARE @lcSucursal NVARCHAR(10)")
            loConsulta.AppendLine("DECLARE @ldFecha DATETIME")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("DECLARE @lcUsuario AS CHAR(10) = " & goServicios.mObtenerCampoFormatoSQL(goUsuario.pcCodigo))
            loConsulta.AppendLine("DECLARE @lcEmpresa AS CHAR(10) = " & goServicios.mObtenerCampoFormatoSQL(goEmpresa.pcCodigo))
            loConsulta.AppendLine("DECLARE @lcEquipo AS CHAR(30) = " & goServicios.mObtenerCampoFormatoSQL(goAuditoria.pcNombreEquipo()))
            loConsulta.AppendLine("")
            '--COLOCAR DOCUMENTO Y RENGLON DE ORIGEN A RENGLÓN DE LA FACTURA DE COMPRA. NO SE PUEDE COLOCAR EL TIPO PORQUE NO VA A COINCIDIR CON LA RECEPCIÓN QUE SE ESTÁ ASOCIANDO.
            loConsulta.AppendLine("UPDATE Renglones_Compras")
            loConsulta.AppendLine("SET Doc_Ori = " & goServicios.mObtenerCampoFormatoSQL(Me.TxtBusqueda.pcTexto("Documento")) & ",")
            loConsulta.AppendLine("    Ren_Ori = " & goServicios.mObtenerCampoFormatoSQL(Me.pcRenglonSelected))
            loConsulta.AppendLine("WHERE Documento = @lcDocumento AND Renglon = @lnRngFact")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("CREATE TABLE #tmpTemporal (	Renglon		INT identity,")
            loConsulta.AppendLine("							    Documento	CHAR(100),")
            loConsulta.AppendLine("							    Fecha		DATETIME,")
            loConsulta.AppendLine("							    Sucursal	CHAR(100),")
            loConsulta.AppendLine("							    Articulo	CHAR(300),")
            loConsulta.AppendLine("							    Nom_art		CHAR(1000),")
            loConsulta.AppendLine("							    Almacen		CHAR(100),")
            loConsulta.AppendLine("							    Cantidad	DECIMAL(28,10),")
            loConsulta.AppendLine("							    Unidad1		CHAR(100),")
            loConsulta.AppendLine("							    Can_uni1	CHAR(130),")
            loConsulta.AppendLine("							    Unidad2		CHAR(10),")
            loConsulta.AppendLine("							    Can_uni2	CHAR(130),")
            loConsulta.AppendLine("							    Precio1		DECIMAL(28,10),")
            loConsulta.AppendLine("							    Precio2		DECIMAL(28,10),")
            loConsulta.AppendLine("							    Max1		DECIMAL(28,10),")
            loConsulta.AppendLine("							    Max2		DECIMAL(28,10))")
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
            '--OBTENER COSTOS Y EXISTENCIA DEL ARTÍCULO
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
            '--CÁLCULO DE COSTO ÚLTIMO Y PROMEDIO
            loConsulta.AppendLine("SET @lnCostoUlt1 = ((@lnCantidadFac * @lnPrecio1) + (@lnCantidadAju * 0)) / (@lnCantidadFac + @lnCantidadAju)")
            loConsulta.AppendLine("SET @lnCostoUlt2 = ((@lnCantidadFac * @lnPrecio2) + (@lnCantidadAju * 0)) / (@lnCantidadFac + @lnCantidadAju)")
            loConsulta.AppendLine("SET @lnCostoPro1 = ((@lnCantidadFac * @lnPrecio1) + (@lnCantidadAju * 0) + (@lnExi_Art * @lnCosPro1_Art)) / (@lnCantidadFac + @lnCantidadAju + @lnExi_Art)")
            loConsulta.AppendLine("SET @lnCostoPro2 = ((@lnCantidadFac * @lnPrecio2) + (@lnCantidadAju * 0) + (@lnExi_Art * @lnCosPro2_Art)) / (@lnCantidadFac + @lnCantidadAju + @lnExi_Art)")
            loConsulta.AppendLine("")
            '--DECLARACIÓN DE DATOS PARA GUARDAR LAS AUDITORIAS DE LA INSERCIÓN DE LOS AJUSTES
            loConsulta.AppendLine("DECLARE @lcAud_Usuario      NVARCHAR(10) = @lcUsuario;")
            loConsulta.AppendLine("DECLARE @lcAud_Tipo         NVARCHAR(15) = 'Datos';")
            loConsulta.AppendLine("DECLARE @lcAud_Tabla        NVARCHAR(30) = 'Ajustes';")
            loConsulta.AppendLine("DECLARE @lcAud_Opcion       NVARCHAR(100) = 'Ajustes_Inventarios';")
            loConsulta.AppendLine("DECLARE @lcAud_Accion       NVARCHAR(10) = 'Agregar';")
            loConsulta.AppendLine("DECLARE @lcAud_Documento    NVARCHAR(10) = '';")
            loConsulta.AppendLine("DECLARE @lcAud_Codigo       NVARCHAR(30) = '';")
            loConsulta.AppendLine("DECLARE @lcAud_Clave2       NVARCHAR(100) = '';")
            loConsulta.AppendLine("DECLARE @lcAud_Clave3       NVARCHAR(100) = '';")
            loConsulta.AppendLine("DECLARE @lcAud_Clave4       NVARCHAR(100) = '';")
            loConsulta.AppendLine("DECLARE @lcAud_Clave5       NVARCHAR(100) = '';")
            loConsulta.AppendLine("DECLARE @lcAud_Detalle      NVARCHAR(MAX) = '';")
            loConsulta.AppendLine("DECLARE @lcAud_Equipo       NVARCHAR(30) = @lcEquipo;")
            loConsulta.AppendLine("DECLARE @lcAud_Sucursal     NVARCHAR(10) = @lcSucursal;")
            loConsulta.AppendLine("DECLARE @lcAud_Objeto       NVARCHAR(100) = 'goEvento';")
            loConsulta.AppendLine("DECLARE @lcAud_Notas        NVARCHAR(MAX) = 'Documento creado automáticamente desde el complemento Asociar recepción a factura de compra';")
            loConsulta.AppendLine("DECLARE @lcAud_Empresa      NVARCHAR(10) = @lcEmpresa;")
            loConsulta.AppendLine("")
            'OBTENER PRÓXIMO CONTADOR
            loConsulta.AppendLine("EXECUTE @RC = [dbo].[mObtenerProximoContador] ")
            loConsulta.AppendLine("			'AJUINV'")
            loConsulta.AppendLine("			,@lcSucursal")
            loConsulta.AppendLine("			,'Normal'")
            loConsulta.AppendLine("			,@lcProximoContador OUTPUT")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("SET @lcAud_Documento = @lcProximoContador")
            loConsulta.AppendLine("")
            '--INSERTAR EL AJUSTE DE COSTOS. ESTOS LLEVAN CUATRO RENGLONES 
            '--1. COSTO ÚLTIMO MONEDA NACIONAL
            '--2. COSTO ÚLTIMO OTRA MONEDA
            '--3. COSTO PROMEDIO MONEDA NACIONAL
            '--4. COSTO PROMEDIO OTRA MONEDA.
            '--EL VALOR SE DEBE GUARDAR EN EL CAMPO cos_ult1 PARA PODER VISUALIZARLO EN EL SISTEMA
            loConsulta.AppendLine("INSERT INTO Ajustes (Documento,Status,Cod_Mon,Tasa,Numerico1,Tipo,Tip_Ori,Doc_Ori,Comentario,Cod_Suc,Fec_Ini,Fec_Fin)")
            loConsulta.AppendLine("VALUES(@lcProximoContador,'Pendiente','VEB',1.00,@lnCantidadAju,'Costo','Compras',@lcDocumento,")
            loConsulta.AppendLine("			'Ajuste de Inventario de costos generado desde compra ' + @lcDocumento ,@lcSucursal,@ldFecha,@ldFecha)")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("INSERT INTO Renglones_Ajustes (Documento,Cod_Art,Renglon,Cod_Tip,Cod_Alm,Can_Art1,Cos_Ult1,--Cos_Ult2,Cos_Pro1,Cos_Pro2,")
            loConsulta.AppendLine("								Mon_Net,tip_ori,doc_ori,ren_ori,Notas,Can_Uni,Cod_Uni,Can_Uni2,Cod_Uni2,Can_Art2,")
            loConsulta.AppendLine("								Ult_Ant1,Ult_Ant2,Cos_Ant1,Cos_Ant2,Tipo)")
            loConsulta.AppendLine("SELECT @lcProximoContador,Articulo,1,'CUMN',almacen,0,@lnCostoUlt1,--@lnCostoUlt2,@lnCostoPro1,@lnCostoPro2,")
            loConsulta.AppendLine("		0,'Compras',@lcDocumento,@lnRngFact,nom_art,can_uni1,unidad1,can_uni2,unidad2,0,")
            loConsulta.AppendLine("		@lnCosUlt1_Art,@lnCosUlt2_Art,@lnCostoUlt1,@lnCostoUlt2,'Cos_Ult1'")
            loConsulta.AppendLine("FROM #tmpTemporal")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("INSERT INTO Renglones_Ajustes (Documento,Cod_Art,Renglon,Cod_Tip,Cod_Alm,Can_Art1,Cos_Ult1,--Cos_Ult2,Cos_Pro1,Cos_Pro2,")
            loConsulta.AppendLine("								Mon_Net,tip_ori,doc_ori,ren_ori,Notas,Can_Uni,Cod_Uni,Can_Uni2,Cod_Uni2,Can_Art2,")
            loConsulta.AppendLine("								Ult_Ant1,Ult_Ant2,Cos_Ant1,Cos_Ant2,Tipo)")
            loConsulta.AppendLine("SELECT @lcProximoContador,Articulo,2,'CUOM',almacen,0,@lnCostoUlt2,--@lnCostoUlt2,@lnCostoPro1,@lnCostoPro2,")
            loConsulta.AppendLine("		0,'Compras',@lcDocumento,@lnRngFact,nom_art,can_uni1,unidad1,can_uni2,unidad2,0,")
            loConsulta.AppendLine("		@lnCosUlt1_Art,@lnCosUlt2_Art,@lnCostoUlt1,@lnCostoUlt2,'Cos_Ult2'")
            loConsulta.AppendLine("FROM #tmpTemporal")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("INSERT INTO Renglones_Ajustes (Documento,Cod_Art,Renglon,Cod_Tip,Cod_Alm,Can_Art1,Cos_Ult1,--Cos_Ult2,Cos_Pro1,Cos_Pro2,")
            loConsulta.AppendLine("								Mon_Net,tip_ori,doc_ori,ren_ori,Notas,Can_Uni,Cod_Uni,Can_Uni2,Cod_Uni2,Can_Art2,")
            loConsulta.AppendLine("								Ult_Ant1,Ult_Ant2,Cos_Ant1,Cos_Ant2,Tipo)")
            loConsulta.AppendLine("SELECT @lcProximoContador,Articulo,3,'CPMN',almacen,0,@lnCostoPro1,--@lnCostoUlt2,@lnCostoPro1,@lnCostoPro2,")
            loConsulta.AppendLine("		0,'Compras',@lcDocumento,@lnRngFact,nom_art,can_uni1,unidad1,can_uni2,unidad2,0,")
            loConsulta.AppendLine("		@lnCosUlt1_Art,@lnCosUlt2_Art,@lnCostoUlt1,@lnCostoUlt2,'Cos_Pro1'")
            loConsulta.AppendLine("FROM #tmpTemporal")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("INSERT INTO Renglones_Ajustes (Documento,Cod_Art,Renglon,Cod_Tip,Cod_Alm,Can_Art1,Cos_Ult1,--Cos_Ult2,Cos_Pro1,Cos_Pro2,")
            loConsulta.AppendLine("								Mon_Net,tip_ori,doc_ori,ren_ori,Notas,Can_Uni,Cod_Uni,Can_Uni2,Cod_Uni2,Can_Art2,")
            loConsulta.AppendLine("								Ult_Ant1,Ult_Ant2,Cos_Ant1,Cos_Ant2,Tipo)")
            loConsulta.AppendLine("SELECT @lcProximoContador,Articulo,4,'CPOM',almacen,0,@lnCostoPro2,--@lnCostoUlt2,@lnCostoPro1,@lnCostoPro2,")
            loConsulta.AppendLine("		0,'Compras',@lcDocumento,@lnRngFact,nom_art,can_uni1,unidad1,can_uni2,unidad2,0,")
            loConsulta.AppendLine("		@lnCosUlt1_Art,@lnCosUlt2_Art,@lnCostoUlt1,@lnCostoUlt2,'Cos_Pro2'")
            loConsulta.AppendLine("FROM #tmpTemporal")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("")
            '--SI SE GUARDÓ EL AJUSTE SE GUARDA LA AUDITORÍA.
            loConsulta.AppendLine("IF EXISTS (SELECT * FROM Ajustes WHERE Documento = @lcProximoContador)")
            loConsulta.AppendLine("BEGIN	")
            loConsulta.AppendLine("	EXECUTE [dbo].[sp_GuardarAuditoria] ")
            loConsulta.AppendLine("            @lcAud_Usuario, @lcAud_Tipo, @lcAud_Tabla, @lcAud_Opcion, @lcAud_Accion,")
            loConsulta.AppendLine("            @lcAud_Documento, @lcAud_Codigo, @lcAud_Clave2, @lcAud_Clave3, @lcAud_Clave4, @lcAud_Clave5,")
            loConsulta.AppendLine("            @lcAud_Detalle, @lcAud_Equipo, @lcAud_Sucursal, @lcAud_Objeto, @lcAud_Notas, @lcAud_Empresa")
            loConsulta.AppendLine("END")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("DROP TABLE #tmpArticulo")
            loConsulta.AppendLine("")
            '--OBTENER PRÓXIMO CONTADOR PARA EL AJUSTE DE EXISTENCIA
            loConsulta.AppendLine("EXECUTE @RC = [dbo].[mObtenerProximoContador] ")
            loConsulta.AppendLine("		'AJUINV'")
            loConsulta.AppendLine("		,@lcSucursal")
            loConsulta.AppendLine("		,'Normal'")
            loConsulta.AppendLine("		,@lcProximoContador OUTPUT")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("SET @lcAud_Documento = @lcProximoContador")
            loConsulta.AppendLine("")
            '--INSERTAR AJUSTE DE EXISTENCIA POR LA CANTIDAD DEL RENGLÓN DE LA FACTURA YA QUE ESTE MATERIAL YA ENTRÓ A INVENTARIO POR LA NOTA DE RECEPCIÓN
            loConsulta.AppendLine("INSERT INTO Ajustes (Documento,status,cod_mon,tasa,tipo,tip_ori,doc_ori,comentario,cod_suc,fec_ini,fec_fin)")
            loConsulta.AppendLine("VALUES(@lcProximoContador,'Pendiente','VEB',1.00,'Existencia','Compras',@lcDocumento,")
            loConsulta.AppendLine("	'Ajuste de Inventario de existencia generado desde compra ' + @lcDocumento,@lcSucursal,@ldFecha,@ldFecha)")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("INSERT INTO renglones_ajustes (Documento,cod_art,renglon,cod_tip,tipo,cod_alm,can_art1,cos_ult1,")
            loConsulta.AppendLine("								mon_net,tip_ori,doc_ori,ren_ori,notas,can_uni,cod_uni,can_uni2,cod_uni2,can_art2,")
            loConsulta.AppendLine("								cos_ult2,ult_ant1,Ult_Ant2,cos_pro1,cos_pro2,cos_ant1,cos_ant2)")
            loConsulta.AppendLine("SELECT @lcProximoContador,Articulo,1,")
            loConsulta.AppendLine("		CASE WHEN @lnCantidadAju > 0 THEN 'E01' ELSE 'S01' END,")
            loConsulta.AppendLine("		CASE WHEN @lnCantidadAju > 0 THEN 'Entrada' ELSE 'Salida' END,")
            loConsulta.AppendLine("		almacen,ABS(@lnCantidadFac),0,ABS(@lnCantidadFac)*0,'Compras',@lcDocumento,@lnRngFact,")
            loConsulta.AppendLine("		nom_art,can_uni1,unidad1,can_uni2,unidad2,ABS(@lnCantidadFac),precio2,precio1,")
            loConsulta.AppendLine("		precio2,precio1,precio2,precio1,precio2")
            loConsulta.AppendLine("FROM #tmpTemporal")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("UPDATE Ajustes SET Can_Art1 = (SELECT SUM(Can_Art1) FROM Renglones_Ajustes WHERE Documento = @lcProximoContador) WHERE Documento = @lcProximoContador")
            loConsulta.AppendLine("")
            '--SI SE GUARDÓ EL AJUSTE SE GUARDA LA AUDITORÍA.
            loConsulta.AppendLine("IF EXISTS (SELECT * FROM Ajustes WHERE Documento = @lcProximoContador)")
            loConsulta.AppendLine("BEGIN")
            loConsulta.AppendLine("	EXECUTE [dbo].[sp_GuardarAuditoria] ")
            loConsulta.AppendLine("            @lcAud_Usuario, @lcAud_Tipo, @lcAud_Tabla, @lcAud_Opcion, @lcAud_Accion,")
            loConsulta.AppendLine("            @lcAud_Documento, @lcAud_Codigo, @lcAud_Clave2, @lcAud_Clave3, @lcAud_Clave4, @lcAud_Clave5,")
            loConsulta.AppendLine("            @lcAud_Detalle, @lcAud_Equipo, @lcAud_Sucursal, @lcAud_Objeto, @lcAud_Notas, @lcAud_Empresa")
            loConsulta.AppendLine("END")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("DROP TABLE #tmpTemporal")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("")
            loConsulta.AppendLine("")

            Try

                loDatos.mEjecutarTransaccion(New String() {loConsulta.ToString()})

                Me.mMostrarMensajeModal("Ajuste Generado", "Se generaron los ajustes de inventario correspondientes.", "i")
                Me.cmdAceptar.Enabled = False
                Me.cmdCancelar.Text = "Cerrar"

            Catch ex As Exception
                Me.mMostrarMensajeModal("Proceso no Completado", "No fue posible crear el ajuste. Información Adicional: <br/>" & ex.Message, "e")
            End Try
        Else
            Me.mMostrarMensajeModal("Proceso no completado", "No ha seleccionado ningún renglón.", "i", True)

        End If
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
        'COLOCAR COMENTARIO 
        Dim lcDocumento As String = goServicios.mObtenerCampoFormatoSQL(Me.TxtBusqueda.pcTexto("Documento"))

        Dim lcConsultaOrden As String = "SELECT Comentario FROM Recepciones WHERE Documento = " & lcDocumento

        Dim loTConsulta As DataTable = (New goDatos()).mObtenerTodosSinEsquema(lcConsultaOrden, "Articulos").Tables(0)

        Me.TxtComentario.Text = CStr(loTConsulta.Rows(0).Item("Comentario")).Trim()

        'LLENA RENGLONES CON LA INFORMACIÓN DE LA RECEPCIÓN
        Dim lcConsulta As New StringBuilder()

        lcConsulta.AppendLine("SELECT Renglones_Recepciones.Renglon     AS Renglon,")
        lcConsulta.AppendLine("       Renglones_Recepciones.Cod_Art     AS Cod_Art,")
        lcConsulta.AppendLine("       Articulos.Nom_Art                 AS Nom_Art,")
        lcConsulta.AppendLine("       Renglones_Recepciones.Can_Art1    AS Can_Art")
        lcConsulta.AppendLine("FROM Renglones_Recepciones")
        lcConsulta.AppendLine(" JOIN Articulos ON Articulos.Cod_Art = Renglones_Recepciones.Cod_Art")
        lcConsulta.AppendLine("WHERE Documento = " & lcDocumento)
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

        For i As Integer = 0 To loRenglones.Rows.Count - 1
            Dim loRenglon As DataRow = loTabla.NewRow()

            loRenglon("Renglon") = CDec(loRenglones.Rows(i).Item("Renglon"))
            loRenglon("cod_art") = CStr(loRenglones.Rows(i).Item("Cod_Art"))
            loRenglon("nom_art") = CStr(loRenglones.Rows(i).Item("Nom_Art"))
            loRenglon("can_art") = CDec(loRenglones.Rows(i).Item("Can_Art"))

            loTabla.Rows.Add(loRenglon)
        Next

        Me.grdRenglones.poOrigenDeDatos = loTabla

        Me.grdRenglones.DataBind()
        Me.grdRenglones.mAlmacenarRenglones()
    End Sub

    Private Sub grdRenglones_mFilaSeleccionada(lnFilaAnterior As Integer, lnFilaNueva As Integer) Handles grdRenglones.mFilaSeleccionada
        Me.pcRenglonSelected = Me.grdRenglones.pnIndiceFilaSeleccionada + 1

        Dim lcConsulta As String = "SELECT Cod_Art, Can_Art1 FROM Renglones_Compras WHERE Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento) & "AND Renglon = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenRenglon)

        Dim loConsulta As DataTable = (New goDatos()).mObtenerTodosSinEsquema(lcConsulta, "Renglones_OCompras").Tables(0)

        Dim lcArticulo As String = Me.grdRenglones.poValorDatos(Me.grdRenglones.pnIndiceFilaSeleccionada, "Cod_Art")
        Dim ldCantidad As Decimal = Me.grdRenglones.poValorDatos(Me.grdRenglones.pnIndiceFilaSeleccionada, "Can_Art")

        'VALIDAR ARTÍCULO SELECCIONADO SEA IGUAL AL DEL RENGLÓN DESDE DONDE SE EJECUTÓ EL COMPLEMENTO
        If (CStr(loConsulta.Rows(0).Item("Cod_Art")).Trim() <> CStr(lcArticulo).Trim()) Then
            Me.mMostrarMensajeModal("Operación no permitida", "El artículo recibido no coincide con los artículos de la orden de compra.", "e", True)
            Me.pcRenglonSelected = 0
        End If

        'VALIDAR CANTIDADES PARA VERIFICAR SI SE REQUIERE LOS AJUSTES DE INVENTARIO
        If (CDec(loConsulta.Rows(0).Item("Can_Art1")) > ldCantidad) Then
            Me.lblAdvertencia.Text = "La cantidad del renglón de la factura de compra es mayor a la recepción. </br> Se generará un ajuste de existencia y un ajuste de costos. <br/> Se asociará este renglón a la recepción " & Me.TxtBusqueda.pcTexto("Documento") & " - renglón " & Me.pcRenglonSelected & ". "
        Else
            Me.mMostrarMensajeModal("Operación no permitida", "Debe agregar la factura desde Anexar Documentos", "e", True)
            Me.pcRenglonSelected = 0
        End If

    End Sub

#End Region

End Class
'-------------------------------------------------------------------------------------------'
' Fin del codigo																			'
'-------------------------------------------------------------------------------------------'
' KDE: 12/12/17: Codigo Inicial.								                            '
'-------------------------------------------------------------------------------------------'
' 
