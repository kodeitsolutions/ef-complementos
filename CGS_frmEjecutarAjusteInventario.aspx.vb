'-------------------------------------------------------------------------------------------'
' Inicio del codigo
'-------------------------------------------------------------------------------------------'
Imports vis3Controles.wbcAdministradorMensajeModal
Imports Microsoft.VisualBasic
Imports cusAplicacion
Imports cusDatos
Imports System.Data

'-------------------------------------------------------------------------------------------'
' Inicio de clase "CGS_frmEjecutarAjusteInventario"
'-------------------------------------------------------------------------------------------'
Partial Class CGS_frmEjecutarAjusteInventario
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
                'Me.pcOrigenRenglon = CStr(laIndices("Renglon")).Trim()

                Me.pcTablaDocumento = CStr(laParametros("lcTabla")).Trim()

                'Me.txtComentario.Text = Me.pcTablaDocumento

                Dim lcRenglonOrigen As Integer = 1 'laIndices("Renglon")
                Me.pcOrigenRenglon = "1"

                Me.mCargarDocumento(Me.pcOrigenDocumento, Me.pcOrigenRenglon)

            End If

        End If

    End Sub

    Protected Sub cmdAceptar_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles cmdAceptar.Click


        Dim lcDocumentoSQL As String = goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento)
        Dim lnDiferencia As Decimal = CDec(Me.txtCan_Rev.pbValor) - CDec(Me.txtCan_Dec.Text)
        Dim loConsulta As New StringBuilder()
        Dim loDatos As New goDatos()

        'PARA VERIFICAR SI HAY UN ERROR 
        'loConsulta.AppendLine("ELSE")
        'loConsulta.AppendLine("BEGIN")
        'loConsulta.AppendLine("    THROW 50000, 'Error!!', 0")
        'loConsulta.AppendLine("END")


        loConsulta.AppendLine("DECLARE @lcDocumento VARCHAR(10) = " & lcDocumentoSQL)
        loConsulta.AppendLine("DECLARE @RC int")
        loConsulta.AppendLine("DECLARE @lcCodigoSucursal nvarchar(10)")
        loConsulta.AppendLine("DECLARE @lcProximoContador nvarchar(10)")
        'loConsulta.AppendLine("DECLARE @ldFecha AS DATETIME = " & goServicios.mObtenerCampoFormatoSQL(Date.Now()))
        loConsulta.AppendLine("DECLARE @ldFecha AS DATETIME ")
        loConsulta.AppendLine("DECLARE @lnCantidad AS DECIMAL (28,2) = " & goServicios.mObtenerCampoFormatoSQL(lnDiferencia))
        loConsulta.AppendLine("DECLARE @lcLote AS VARCHAR(30)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("CREATE TABLE #tmpTemporal (	Renglon		int identity,")
        loConsulta.AppendLine("							Documento	CHAR(100),")
        loConsulta.AppendLine("							Fecha		DATETIME,")
        loConsulta.AppendLine("							Sucursal	CHAR(100),")
        loConsulta.AppendLine("							Articulo	CHAR(300),")
        loConsulta.AppendLine("							Nom_art		CHAR(1000),")
        loConsulta.AppendLine("							almacen		CHAR(100),")
        loConsulta.AppendLine("							unidad1		CHAR(100),")
        loConsulta.AppendLine("							can_uni1	CHAR(130),")
        loConsulta.AppendLine("							unidad2		CHAR(10),")
        loConsulta.AppendLine("							can_uni2	CHAR(130),")
        loConsulta.AppendLine("							Precio1		DECIMAL(28,10),")
        loConsulta.AppendLine("							Precio2		DECIMAL(28,10),")
        loConsulta.AppendLine("							Max1		DECIMAL(28,10),")
        loConsulta.AppendLine("							Max2		DECIMAL(28,10),")
        loConsulta.AppendLine("							Lote		CHAR(30),")
        loConsulta.AppendLine("							Cant_Lot	DECIMAL(28,10))")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("INSERT INTO #tmpTemporal(Documento, Fecha, Articulo,nom_art,almacen,unidad1,can_uni1, unidad2,")
        loConsulta.AppendLine("						can_uni2,Precio1,Precio2,Max1,Max2,Sucursal, Lote, Cant_Lot)")
        loConsulta.AppendLine("SELECT	Renglones_Recepciones.documento,")
        loConsulta.AppendLine("		Recepciones.Fec_Ini,")
        loConsulta.AppendLine("		Renglones_Recepciones.cod_art,")
        loConsulta.AppendLine("		Articulos.nom_art,")
        loConsulta.AppendLine("		Renglones_Recepciones.cod_alm,")
        loConsulta.AppendLine("		Renglones_Recepciones.cod_uni,")
        loConsulta.AppendLine("		Renglones_Recepciones.can_uni,")
        loConsulta.AppendLine("		Renglones_Recepciones.cod_uni2,")
        loConsulta.AppendLine("		Renglones_Recepciones.can_uni2,")
        loConsulta.AppendLine("		Renglones_Recepciones.precio1,")
        loConsulta.AppendLine("		Renglones_Recepciones.precio2,")
        loConsulta.AppendLine("		MAX(Renglones_Recepciones.precio1) OVER(Partition by Articulos.cod_art),")
        loConsulta.AppendLine("		MAX(Renglones_Recepciones.precio2) OVER(Partition by Articulos.cod_art),")
        loConsulta.AppendLine("		Recepciones.cod_suc,")
        loConsulta.AppendLine("		COALESCE(Operaciones_lotes.Cod_Lot, ''),")
        loConsulta.AppendLine("		COALESCE(SUM(Operaciones_Lotes.Cantidad), 0)")
        loConsulta.AppendLine("FROM Recepciones")
        loConsulta.AppendLine("	JOIN Renglones_Recepciones ON Renglones_Recepciones.Documento = Recepciones.documento")
        loConsulta.AppendLine("	JOIN Articulos ON Articulos.cod_art = Renglones_Recepciones.cod_art")
        loConsulta.AppendLine("	LEFT JOIN Operaciones_Lotes ON Operaciones_Lotes.Num_Doc = Renglones_Recepciones.Documento")
        loConsulta.AppendLine("        AND Operaciones_Lotes.Tip_Doc = 'Recepciones' ")
        loConsulta.AppendLine("		AND Operaciones_Lotes.Ren_Ori = Renglones_Recepciones.Renglon")
        loConsulta.AppendLine("		AND Operaciones_Lotes.Tip_Ope = 'Entrada'")
        loConsulta.AppendLine("WHERE Renglones_Recepciones.documento = @lcDocumento")
        loConsulta.AppendLine("GROUP BY Renglones_Recepciones.documento,Recepciones.Fec_Ini,Renglones_Recepciones.cod_art,Articulos.nom_art,Renglones_Recepciones.cod_alm,")
        loConsulta.AppendLine("		Renglones_Recepciones.cod_uni,Renglones_Recepciones.can_uni,Renglones_Recepciones.cod_uni2,Renglones_Recepciones.can_uni2,")
        loConsulta.AppendLine("		Renglones_Recepciones.precio1,Renglones_Recepciones.precio2,Recepciones.cod_suc,Operaciones_lotes.Cod_Lot, Articulos.Cod_Art")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SET @lcCodigoSucursal = (SELECT Sucursal FROM #tmpTemporal);")
        loConsulta.AppendLine("SET @ldFecha = (SELECT Fecha FROM #tmpTemporal)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("EXECUTE @RC = [dbo].[mObtenerProximoContador] ")
        loConsulta.AppendLine("		'AJUINV'")
        loConsulta.AppendLine("		,@lcCodigoSucursal")
        loConsulta.AppendLine("		,'Normal'")
        loConsulta.AppendLine("		,@lcProximoContador OUTPUT")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("INSERT INTO Ajustes (Documento,status,automatico,cod_mon,tasa,tipo,tip_ori,doc_ori,comentario,cod_suc,fec_ini,fec_fin)")
        loConsulta.AppendLine("VALUES(@lcProximoContador,'Confirmado',1,'VEB',1.00,'Existencia','Recepciones',@lcDocumento,")
        loConsulta.AppendLine("	'Ajuste de Inventario generado automáticamente desde complemento ''Ajuste de existencia por cantidad revisada'' ',@lcCodigoSucursal,@ldFecha,@ldFecha)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("INSERT INTO renglones_ajustes (Documento,cod_art,renglon,cod_tip,tipo,cod_alm,can_art1,cos_ult1,")
        loConsulta.AppendLine("								mon_net,tip_ori,doc_ori,notas,can_uni,cod_uni,can_uni2,cod_uni2,can_art2,")
        loConsulta.AppendLine("								cos_ult2,ult_ant1,Ult_Ant2,cos_pro1,cos_pro2,cos_ant1,cos_ant2)")
        loConsulta.AppendLine("SELECT @lcProximoContador,Articulo,1,")
        loConsulta.AppendLine("		CASE WHEN @lnCantidad > 0 THEN 'E01' ELSE 'S01' END,")
        loConsulta.AppendLine("		CASE WHEN @lnCantidad > 0 THEN 'Entrada' ELSE 'Salida' END,")
        loConsulta.AppendLine("		almacen,ABS(@lnCantidad),0,ABS(@lnCantidad)*0,'Recepciones',@lcDocumento,")
        loConsulta.AppendLine("		nom_art,can_uni1,unidad1,can_uni2,unidad2,ABS(@lnCantidad),precio2,precio1,")
        loConsulta.AppendLine("		precio2,precio1,precio2,precio1,precio2")
        loConsulta.AppendLine("FROM #tmpTemporal")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lcArticulo AS VARCHAR(8) = (SELECT Articulo FROM #tmpTemporal)")
        loConsulta.AppendLine("DECLARE @lcAlmacen AS VARCHAR(15) = (SELECT almacen FROM #tmpTemporal)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("UPDATE Articulos SET Exi_Act1 = Exi_Act1 + @lnCantidad WHERE Cod_Art = @lcArticulo")
        loConsulta.AppendLine("UPDATE Renglones_Almacenes SET Exi_Act1 = Exi_Act1  + @lnCantidad ")
        loConsulta.AppendLine("WHERE Cod_ALm = @lcAlmacen AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SET @lcLote = (SELECT Lote FROM #tmpTemporal)")
        loConsulta.AppendLine("IF @lcLote <> ''")
        loConsulta.AppendLine("BEGIN")
        loConsulta.AppendLine("	UPDATE Renglones_Lotes SET Exi_Act1 = Exi_Act1 + @lnCantidad ")
        loConsulta.AppendLine("	WHERE Cod_Lot = @lcLote AND Cod_Alm = @lcAlmacen AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("")
        loConsulta.AppendLine(" UPDATE Lotes SET Exi_Act1 = Exi_Act1 + @lnCantidad WHERE Cod_Lot = @lcLote AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("	INSERT INTO Operaciones_Lotes (Cod_Alm, Cod_Art, Cod_Lot, Cantidad, Num_Doc, Renglon, Tip_Doc, Tip_Ope, Ren_Ori)")
        loConsulta.AppendLine("	SELECT	 Almacen, Articulo, Lote, ABS(@lnCantidad), @lcProximoContador, 1, 'Ajustes_Inventarios',")
        loConsulta.AppendLine("		CASE WHEN @lnCantidad > 0 THEN 'Entrada' ELSE 'Salida' END,1")
        loConsulta.AppendLine("	FROM #tmpTemporal")
        loConsulta.AppendLine("END")
        loConsulta.AppendLine("")
        'loConsulta.AppendLine("UPDATE Ajustes SET Mon_Net = (SELECT SUM(Mon_Net) FROM Renglones_Ajustes WHERE Documento = @lcProximoContador) WHERE Documento = @lcProximoContador")
        loConsulta.AppendLine("UPDATE Ajustes SET Can_Art1 = (SELECT SUM(Can_Art1) FROM Renglones_Ajustes WHERE Documento = @lcProximoContador) WHERE Documento = @lcProximoContador")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("UPDATE Renglones_Recepciones SET Caracter2 = 'AJUSTADO' WHERE Documento = @lcDocumento AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DROP TABLE #tmpTemporal")
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
            lcNombreOpcion = "'CGS_frmEjecutarAjusteInventario.aspx'"
            lcAccion = "'Agregar'"
            lcDocumento = goServicios.mObtenerCampoFormatoSQL(lcDocumentoAjuste)
            lcCodigo = "'Sin código'"
            lcDetalles = goServicios.mObtenerCampoFormatoSQL(goAuditoria.KC_DetalleVacio)
            lcNombreEquipo = goServicios.mObtenerCampoFormatoSQL(goAuditoria.pcNombreEquipo())
            lcCodigoObjeto = goServicios.mObtenerCampoFormatoSQL(TypeName(Me))
            lcNotasRegistro = "'Documento Agregado desde Complemento ""Ajuste de Inventario por cantidad revisada (CGS)""'"
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
    Private Sub mCargarDocumento(ByVal lcDocumento As String, ByVal lcRenglon As Integer)

        Dim lcDocumentoSQL As String = goServicios.mObtenerCampoFormatoSQL(lcDocumento)
        Dim lcRenglonSQL As String = goServicios.mObtenerCampoFormatoSQL(lcRenglon)

        Me.txtDocumento.Text = lcDocumento

        Dim loDatosBusqueda As New goDatos()
        Dim loQuery As New StringBuilder()

        loQuery.AppendLine("SELECT Documento, Comentario, Status")
        loQuery.AppendLine("FROM Recepciones")
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

        loConsulta.AppendLine("SELECT Renglones_Recepciones.Cod_Art         AS Cod_Art, ")
        loConsulta.AppendLine("     SUM(Renglones_Recepciones.Can_Art1)     AS Cantidad,")
        loConsulta.AppendLine("     COALESCE(Operaciones_Lotes.Cod_Lot,'')  AS Lote,")
        loConsulta.AppendLine("     Renglones_Recepciones.Caracter2         AS Estatus")
        loConsulta.AppendLine("FROM Renglones_Recepciones")
        loConsulta.AppendLine(" LEFT JOIN Operaciones_Lotes ON Operaciones_Lotes.Num_Doc = Renglones_Recepciones.Documento")
        loConsulta.AppendLine("     AND Renglones_Recepciones.Cod_Art = Operaciones_Lotes.Cod_Art")
        loConsulta.AppendLine("     AND Operaciones_Lotes.Ren_Ori = Renglones_Recepciones.Renglon")
        loConsulta.AppendLine("     AND Operaciones_Lotes.Tip_Doc = 'Recepciones' AND Operaciones_Lotes.Tip_Ope = 'Entrada'")
        loConsulta.AppendLine("WHERE Renglones_Recepciones.Documento = " & lcDocumentoSQL)
        loConsulta.AppendLine("     AND Renglones_Recepciones.Renglon = " & lcRenglonSQL)
        loConsulta.AppendLine("GROUP BY Renglones_Recepciones.Cod_Art, Operaciones_Lotes.Cod_Lot, Renglones_Recepciones.Caracter2")
        loConsulta.AppendLine("")

        Dim loDatosConsulta As New goDatos()

        Dim loTablaConsulta As DataTable = loDatosConsulta.mObtenerTodosSinEsquema(loConsulta.ToString(), "Renglones_Recepciones").Tables(0)

        Dim loFilaConsulta As DataRow
        If loTablaConsulta.Rows().Count > 0 Then
            loFilaConsulta = loTablaConsulta.Rows(0)
            Me.txtCan_Dec.pbValor = goServicios.mObtenerFormatoCadena(CDec(loFilaConsulta("Cantidad")), goServicios.enuOpcionesRedondeo.KN_RedondeoPuntoMedio, goOpciones.pnDecimalesParaMonto)
            Me.txtAjs_Dif.pbValor = 0
        End If

        'Validaciones:
        If (CStr(loFilaConsulta("Estatus")).Trim() = "AJUSTADO") Then
            Me.mMostrarMensajeModal("Documento Bloqueado", "Este renglón ya tiene un Ajuste de Inventario asociado.", "a")

            Me.mDeshabilitarTodo()
            Return
        End If
        'VALIDACION DE TIPO DE SERVICIO ¡¡¡QUITAR COMENTARIO!!!
        'Dim lcConsulta As String = "SELECT Tipo, Nom_Art FROM Articulos WHERE Cod_Art = " & goServicios.mObtenerCampoFormatoSQL(CStr(loFilaConsulta("Cod_Art")).Trim())
        'Dim loDConsulta As New goDatos()
        'Dim loTConsulta As DataTable = loDConsulta.mObtenerTodosSinEsquema(lcConsulta, "Articulos").Tables(0)

        'If (CStr(loTConsulta.Rows(0).Item("Tipo")).Trim() = "Servicio") Then
        '    Me.mMostrarMensajeModal("Documento bloqueado", "El artículo " & CStr(loFilaConsulta("Cod_Art")).Trim() & " - " & CStr(loTConsulta.Rows(0).Item("Nom_Art")).Trim() & " es de tipo 'Servicio' y no permite Ajustes de Inventario.", "e")

        '    Me.mDeshabilitarTodo()
        '    Return
        'End If

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

#End Region


End Class
''-------------------------------------------------------------------------------------------'
'' Fin del codigo																			'
''-------------------------------------------------------------------------------------------'
'' kode it: 06/06/17: Codigo Inicial.								                            '
''-------------------------------------------------------------------------------------------'
