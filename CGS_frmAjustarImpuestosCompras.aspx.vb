'-------------------------------------------------------------------------------------------'
' Inicio del codigo
'-------------------------------------------------------------------------------------------'
Imports cusAdministrativo

'-------------------------------------------------------------------------------------------'
' Inicio de clase "CGS_frmAjustarImpuestosCompras"
'-------------------------------------------------------------------------------------------'
Partial Class CGS_frmAjustarImpuestosCompras
	Inherits  vis2formularios.frmFormularioGenerico

#Region "Declaraciones"

#End Region

#Region "Propiedades"

''' <summary>
''' Devuelve\establece el valor actual de la tasa en moneda adicional.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
	Private Property pnTasaMonedaAdicional() As Decimal 
		Get
			Return CDec(Me.ViewState("pnTasaMonedaAdicional"))
		End Get
		Set(ByVal lnNuevoValor As Decimal)
			Me.ViewState("pnTasaMonedaAdicional") = lnNuevoValor
		End Set
	End Property

''' <summary>
''' Almacena el número de decimales que seran usados en el cálculo de  montos.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
	Protected Property pnDecimalesMonto() As Integer 
		Get
			Return CInt(Me.ViewState("pnDecimalesMonto"))
		End Get
		Set(ByVal lnNuevoValor As Integer)
			Me.ViewState("pnDecimalesMonto") = lnNuevoValor
		End Set
	End Property
 	
''' <summary>
''' Almacena el número de decimales que seran usados en el cálculo de Cantidades.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
	Protected Property pnDecimalesCantidad() As Integer 
		Get
			Return CInt(Me.ViewState("pnDecimalesCantidad"))
		End Get
		Set(ByVal lnNuevoValor As Integer)
			Me.ViewState("pnDecimalesCantidad") = lnNuevoValor
		End Set
	End Property
 	
    ''' <summary>
    ''' Almacena los renglones originales de artículos cargados. 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property poRenglonesOriginales As DataTable
        Get 
            Return Me.ViewState("poRenglonesOriginales")
        End Get
        Set(value As DataTable)
            Me.ViewState("poRenglonesOriginales") = value
        End Set
    End Property
 	
    ''' <summary>
    ''' Almacena los renglones nuevos de artículos cargados. 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property poRenglonesNuevos As DataTable
        Get 
            Return Me.ViewState("poRenglonesNuevos")
        End Get
        Set(value As DataTable)
            Me.ViewState("poRenglonesNuevos") = value
        End Set
    End Property

    Private Property pnPorcentajeDescuentoDocumento As Decimal 
        Get
            Return CDec(Me.ViewState("pnPorcentajeDescuentoDocumento"))
        End Get
        Set(value As Decimal)
            Me.ViewState("pnPorcentajeDescuentoDocumento") = value
        End Set
    End Property

    Private Property pnPorcentajeRecargoDocumento As Decimal 
        Get
            Return CDec(Me.ViewState("pnPorcentajeRecargoDocumento"))
        End Get
        Set(value As Decimal)
            Me.ViewState("pnPorcentajeRecargoDocumento") = value
        End Set
    End Property

    Private Property pnMontoOtrosDocumento As Decimal 
        Get
            Return CDec(Me.ViewState("pnMontoOtrosDocumento"))
        End Get
        Set(value As Decimal)
            Me.ViewState("pnMontoOtrosDocumento") = value
        End Set
    End Property

    Private Property paParametros As Generic.Dictionary(Of String, Object)
        Get 
            Return Me.ViewState("paParametros")
        End Get
        Set(value As Generic.Dictionary(Of String, Object))
            Me.ViewState("paParametros") = value
        End Set
    End Property  

#End Region

#Region "Metodos"

    ''' <summary>
    ''' Busca la información de impuestos de los renglones del documento indicado. 
    ''' </summary>
    ''' <param name="lcDocumento">Número de documento a cargar.</param>
    ''' <param name="lcTabla">Tabla de origen del documento.</param>
    ''' <remarks></remarks>
    Private Sub mCargarImpuestos(lcDocumento As String, lcTabla As String)

        Dim loRenglones As DataTable = Nothing

        Dim loConsulta As New StringBuilder()
        Select Case lcTabla.ToLower()
            Case "compras"
                loConsulta.AppendLine("SELECT   Compras.Status, ")
                loConsulta.AppendLine("         Compras.Imp_Fij, ")
                loConsulta.AppendLine("         Renglones_Compras.Renglon, ")
                loConsulta.AppendLine("         Renglones_Compras.Por_Imp1,")
                loConsulta.AppendLine("         Renglones_Compras.Mon_Imp1,")
                loConsulta.AppendLine("         Renglones_Compras.Cod_Imp,")
                loConsulta.AppendLine("         ROUND(Renglones_Compras.Mon_Net*(100 + Compras.Por_Rec1 - Compras.Por_Des1)/100, " & Me.pnDecimalesMonto & ") Mon_Net,")
                loConsulta.AppendLine("         Compras.Por_Des1                       AS Descuento_Encabezado,")
                loConsulta.AppendLine("         Compras.Por_Rec1                       AS Recargo_Encabezado,")
                loConsulta.AppendLine("         Compras.Mon_Otr1 + Compras.Mon_Otr2 + Compras.Mon_Otr3   AS Otros_Encabezado")
                loConsulta.AppendLine("FROM     Renglones_Compras")
                loConsulta.AppendLine("    JOIN Compras")
                loConsulta.AppendLine("      ON Compras.Documento = Renglones_Compras.Documento")
                loConsulta.AppendLine("WHERE    Compras.Documento =" & goServicios.mObtenerCampoFormatoSQL(lcDocumento))
            Case "ordenes_compras"
                loConsulta.AppendLine("SELECT   Ordenes_Compras.Status, ")
                loConsulta.AppendLine("         Ordenes_Compras.Imp_Fij, ")
                loConsulta.AppendLine("         Renglones_OCompras.Renglon, ")
                loConsulta.AppendLine("         Renglones_OCompras.Por_Imp1,")
                loConsulta.AppendLine("         Renglones_OCompras.Mon_Imp1,")
                loConsulta.AppendLine("         Renglones_OCompras.Cod_Imp,")
                loConsulta.AppendLine("         ROUND(Renglones_OCompras.Mon_Net*(100 + Ordenes_Compras.Por_Rec1 - Ordenes_Compras.Por_Des1)/100, " & Me.pnDecimalesMonto & ") Mon_Net,")
                loConsulta.AppendLine("         Ordenes_Compras.Por_Des1                     AS Descuento_Encabezado,")
                loConsulta.AppendLine("         Ordenes_Compras.Por_Rec1                     AS Recargo_Encabezado,")
                loConsulta.AppendLine("         Ordenes_Compras.Mon_Otr1 + Ordenes_Compras.Mon_Otr2 + Ordenes_Compras.Mon_Otr3 AS Otros_Encabezado")
                loConsulta.AppendLine("FROM     Renglones_OCompras")
                loConsulta.AppendLine("    JOIN Ordenes_Compras")
                loConsulta.AppendLine("      ON Ordenes_Compras.Documento = Renglones_OCompras.Documento")
                loConsulta.AppendLine("WHERE    Ordenes_Compras.Documento =" & goServicios.mObtenerCampoFormatoSQL(lcDocumento))
        End Select


        Try
            loRenglones = (New goDatos()).mObtenerTodosSinEsquema(loConsulta.ToString(), "").Tables(0)
            Me.poRenglonesOriginales = loRenglones.Copy()
        Catch ex As Exception
            Me.wbcAdministradorMensajeModal.mMostrarMensajeModal("Documento no Encontrado", _
                                                                 "No fue posible cargar los datos del documento """ & _
                                                                 lcDocumento & """ (" & lcTabla & "). Información Adicional: <br/>" & _
                                                                 ex.Message, KN_Advertencia)
            Me.poRenglonesOriginales = Nothing
        End Try

        'En caso de error en la consulta:
		If loRenglones Is Nothing Then 

            Me.mBloquearFormulario()

            Return 

        End If 
		
        'Si el documento no existe:
        If loRenglones.Rows.Count = 0 Then 

            Me.wbcAdministradorMensajeModal.mMostrarMensajeModal("Documento no Encontrado", _
                                                                 "No fue posible encontrar el documento """ & _
                                                                 lcDocumento & """ (" & lcTabla & ").", KN_Advertencia)
            Me.mBloquearFormulario()
            Return
        End If

        'Si el documento no está pendiente:
        If CStr(loRenglones.Rows(0).Item("Status")).Trim().ToLower() <> "pendiente" Then 

            Me.wbcAdministradorMensajeModal.mMostrarMensajeModal("Documento no Válido", _
                                                                 "Solo se puede cambiar el impuesto a un documento con estatus ""Pendiente"". ", _
                                                                 KN_Advertencia)
            Me.mBloquearFormulario()
            Return
        End If

        'Si el documento no tiene impuesto fijo:
        If CBool(loRenglones.Rows(0).Item("Imp_Fij")) Then 

            Me.wbcAdministradorMensajeModal.mMostrarMensajeModal("Documento no Válido", _
                                                                 "No se puede cambiar el impuesto a un documento con impuesto fijo. ", _
                                                                 KN_Advertencia)
            Me.mBloquearFormulario()
            Return
        End If

        Dim lcAnterior As string = goOpciones.mObtener("CODIMPTGEN").Trim() 
        Dim lcNuevo As string = goOpciones.mObtener("CODIMPTGNE").Trim() 

        Me.pnMontoOtrosDocumento = CDec(loRenglones.Rows(0)("Otros_Encabezado"))
        

        'Impuestos iniciales
        Dim laImpuestos As New ArrayList()

        For Each loRenglon As DataRow In loRenglones.Select("", "Por_Imp1 DESC, Cod_Imp DESC")

            Dim lcImpuesto As String = CStr(loRenglon("Cod_Imp")).Trim() 
                
            If Not laImpuestos.Contains(lcImpuesto) Then
                laImpuestos.Add(lcImpuesto)
                Me.cboImpuestoAnterior.Items.Add(lcImpuesto)

            End If

        Next

        Me.cboImpuestoAnterior.SelectedValue = lcAnterior

        'Impuestos finales
        loConsulta.Length = 0

        loConsulta.AppendLine("SELECT    RTRIM(Cod_Imp) Cod_Imp")
        loConsulta.AppendLine("FROM      Impuestos")
        loConsulta.AppendLine("WHERE     Status = 'A'")
        loConsulta.AppendLine("ORDER BY  Cod_Imp ASC")
        loConsulta.AppendLine("")

        Dim loListaImpuestos As DataSet

        loListaImpuestos = (new goDatos()).mObtenerTodosSinEsquema(loConsulta.ToString(), "Impuestos")

        Me.cboImpuestoNuevo.mLlenarLista(loListaImpuestos)

        Dim lcImpuestoNuevo As String = CStr(goOpciones.mObtener("CODIMPTGNE")).Trim()
        Me.cboImpuestoNuevo.SelectedValue = lcImpuestoNuevo

        Me.mCargarDatos(loRenglones)

    End Sub

''' <summary>
''' Carga en pantalla los datos de los renglones de origen. 
''' </summary>
''' <param name="loRenglones"></param>
''' <remarks></remarks>
    Private Sub mCargarDatos(loRenglones As DataTable)
        

		If loRenglones Is Nothing Then 

            Me.mBloquearFormulario()
            Return 

        End If 
        

        Dim lcAnterior As string = Me.cboImpuestoAnterior.SelectedValue.Trim()
        Dim lcNuevo As string = Me.cboImpuestoNuevo.SelectedValue.Trim() 
        Dim lnPorcentajeNuevo As Decimal = goImpuestos.mObtenerPorcentaje(lcNuevo, 10, "")
        
        'Crea una copia para no modificar los originales
        loRenglones = loRenglones.Copy()
        loRenglones.Columns.Add("Cod_Imp_Anterior", GetType(String))

        For Each loRenglon As DataRow In loRenglones.Rows
            Dim lcImpuesto As String = CStr(loRenglon("Cod_Imp")).Trim() 

            loRenglon("Cod_Imp_Anterior") = loRenglon("Cod_Imp")
            If lcImpuesto = lcAnterior Then
                loRenglon("Cod_Imp") = lcNuevo
                loRenglon("Por_Imp1") = lnPorcentajeNuevo
                loRenglon("Mon_Imp1") =  goServicios.mRedondearValor(CDec(loRenglon("Mon_Net"))*lnPorcentajeNuevo/100D, Me.pnDecimalesMonto)
            End If

        Next

        'NOTA: No se indica el porcentaje de descuento o recargo, porque ya están incluidos en el monto neto
        Me.txtImpuesto.pbValor = goImpuestos.mCalcularImpuesto(loRenglones, 0D, 0D)


        'Resumen de impuestos
        Dim laImpuestos As New ArrayList()
        Dim loImpuestos As New DataTable("Impuestos")

        loImpuestos.Columns.Add("Cod_Imp_Anterior", GetType(String))
        loImpuestos.Columns.Add("Cod_Imp", GetType(String))
        loImpuestos.Columns.Add("Por_Imp", GetType(Decimal))
        loImpuestos.Columns.Add("Mon_Imp", GetType(Decimal))
        loImpuestos.Columns.Add("Mon_Net", GetType(Decimal))

        For Each loRenglon As DataRow In loRenglones.Rows

            Dim lcImpuesto As String = CStr(loRenglon("Cod_Imp")).Trim() 
                
            If Not laImpuestos.Contains(lcImpuesto) Then
                laImpuestos.Add(lcImpuesto)

                Dim lnPorcentaje As Decimal = CDec(loRenglon("Por_Imp1"))
                Dim lnImpuesto As Decimal = CDec(loRenglon("Mon_Imp1"))
                Dim lnBruto As Decimal = CDec(loRenglon("Mon_Net"))

                loImpuestos.Rows.Add(CStr(loRenglon("Cod_Imp_Anterior")).Trim(), lcImpuesto, lnPorcentaje, lnImpuesto, lnBruto)

            Else

                Dim loRenglonNuevo As DataRow = loImpuestos.Select("Cod_Imp = " & goServicios.mObtenerCampoFormatoSQL(lcImpuesto))(0)

                loRenglonNuevo("Mon_Imp") += CDec(loRenglon("Mon_Imp1"))
                loRenglonNuevo("Mon_Net") += CDec(loRenglon("Mon_Net"))

            End If

        Next
		
		Me.grdImpuestos.DataSource = loImpuestos
		Me.grdImpuestos.DataBind()
        
        Me.poRenglonesNuevos = loRenglones

        Me.mCalcularTotales()

        If lcAnterior = lcNuevo Then 
            Me.cmdAceptar.Enabled = False
        Else
            Me.cmdAceptar.Enabled = True
        End If

    End Sub

    Private Sub mCalcularTotales()

		Dim loRenglones As DataTable = Me.grdImpuestos.DataSource
		If (loRenglones.Rows.Count > 0) Then
			
			Me.txtNeto.pbValor	    = loRenglones.Compute("SUM(mon_net)","")
			'Me.txtImpuesto.pbValor	= loRenglones.Compute("SUM(mon_imp)","")
			Me.txtOtros.pbValor	    = Me.pnMontoOtrosDocumento
			Me.txtTotal.pbValor		= loRenglones.Compute("SUM(mon_imp)+SUM(mon_net)","") + Me.pnMontoOtrosDocumento
		Else
			
			Me.txtNeto.pbValor	    = 0D
			Me.txtImpuesto.pbValor	= 0D
			Me.txtTotal.pbValor		= 0D
			
		End If       

    End Sub

    Private Sub mBloquearFormulario()

        Me.cmdAceptar.Visible = False
        Me.cmdAceptar.Enabled = False

        Me.cboImpuestoAnterior.Enabled = False
        Me.cboImpuestoNuevo.Enabled = False

    End Sub

    Private Sub mGuardarCambios()

        Dim loRenglones As DataTable = Me.poRenglonesNuevos


        Dim lcDocumento As String = DirectCast(Me.paParametros("laIndices"), Generic.Dictionary(Of String, Object))("Documento")

        Dim lcDecimal28_X As String = "Decimal(28," & Me.pnDecimalesMonto.ToString() & ")"
        Dim lnTotalImpuesto As Decimal = Me.txtImpuesto.pbValor

        Dim lcTabla As String = CStr(Me.paParametros("lcTabla"))
        Dim lcCondicionSalida As String = CStr(Me.paParametros("lcCondicion"))
        Dim lcTablaEncabezado As String = ""
        Dim lcTablaRenglones As String = ""
        Dim lcFormularioSalida As String = ""
        Select Case lcTabla.ToLower()
            Case "compras"
                lcTablaEncabezado = "[Compras]"
                lcTablaRenglones = "[Renglones_Compras]"
                lcFormularioSalida = "../../Administrativo/Formularios/frmOperacionFacturasCompra.aspx"
            Case "ordenes_compras"
                lcTablaEncabezado = "[Ordenes_Compras]"
                lcTablaRenglones = "[Renglones_OCompras]"
                lcFormularioSalida = "../../Administrativo/Formularios/frmOperacionOrdenesCompra.aspx"
            Case Else
                Return
        End Select

        Dim loConsulta As New StringBuilder()



        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lcDocumento CHAR(10);")
        loConsulta.AppendLine("SET @lcDocumento = "  & goServicios.mObtenerCampoFormatoSQL(lcDocumento) & ";")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("--Tablas temporales de valores originales para las auditorias")
        loConsulta.AppendLine("CREATE TABLE #tblEncabezadoOri( Mon_Imp1 DECIMAL(28, 10),")
        loConsulta.AppendLine("                                Cod_Imp1 CHAR(10) COLLATE DATABASE_DEFAULT,")
        loConsulta.AppendLine("                                Cod_Imp2 CHAR(10) COLLATE DATABASE_DEFAULT,")
        loConsulta.AppendLine("                                Cod_Imp3 CHAR(10) COLLATE DATABASE_DEFAULT,")
        loConsulta.AppendLine("                                Por_Imp1 DECIMAL(28, 10),")
        loConsulta.AppendLine("                                Por_Imp2 DECIMAL(28, 10),")
        loConsulta.AppendLine("                                Por_Imp3 DECIMAL(28, 10),")
        loConsulta.AppendLine("                                Mon_Net  DECIMAL(28, 10),")
        loConsulta.AppendLine("                                Dis_Imp  VARCHAR(MAX) COLLATE DATABASE_DEFAULT);")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("CREATE TABLE #tblRenglonesOri(  Renglon INT,")
        loConsulta.AppendLine("                                Por_Imp1 DECIMAL(28, 10),")
        loConsulta.AppendLine("                                Mon_Imp1 DECIMAL(28, 10),")
        loConsulta.AppendLine("                                Cod_Imp CHAR(10) COLLATE DATABASE_DEFAULT);")
        loConsulta.AppendLine("                            ")
        loConsulta.AppendLine("INSERT INTO #tblRenglonesOri")
        loConsulta.AppendLine("SELECT  Renglon, Por_Imp1, Mon_Imp1, Cod_Imp")
        loConsulta.AppendLine("FROM    " & lcTablaRenglones)
        loConsulta.AppendLine("WHERE   Documento = @lcDocumento;")
        loConsulta.AppendLine("                   ")
        loConsulta.AppendLine("INSERT INTO #tblEncabezadoOri")
        loConsulta.AppendLine("SELECT  Mon_Imp1, Cod_Imp1, Cod_Imp2, Cod_Imp3, Por_Imp1, Por_Imp2, Por_Imp3, Mon_Net, Dis_Imp")
        loConsulta.AppendLine("FROM    " & lcTablaEncabezado)
        loConsulta.AppendLine("WHERE   Documento = @lcDocumento;")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("-- *****************************************************")
        loConsulta.AppendLine("-- Valores a cambiar")
        loConsulta.AppendLine("-- *****************************************************")
        loConsulta.AppendLine("CREATE TABLE #tblRenglones( Renglon INT,")
        loConsulta.AppendLine("                            Por_Imp1 DECIMAL(28, 10),")
        loConsulta.AppendLine("                            Mon_Imp1 DECIMAL(28, 10),")
        loConsulta.AppendLine("                            Cod_Imp CHAR(10) COLLATE DATABASE_DEFAULT,")
        loConsulta.AppendLine("                            Cambio BIT);")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")

        For Each loRenglon As DataRow In poRenglonesNuevos.Rows

            loConsulta.Append("INSERT INTO #tblRenglones VALUES(")
            loConsulta.Append(CInt(loRenglon("Renglon")))
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(loRenglon("Por_Imp1"), goServicios.enuOpcionesRedondeo.KN_RedondeoPuntoMedio, 10))
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(loRenglon("Mon_Imp1"), goServicios.enuOpcionesRedondeo.KN_RedondeoPuntoMedio, 10))
            loConsulta.Append(",")
            loConsulta.Append(goServicios.mObtenerCampoFormatoSQL(loRenglon("Cod_Imp")))
            loConsulta.Append(",")
            loConsulta.Append(IIf(loRenglon("Cod_Imp") <> loRenglon("Cod_Imp_Anterior"), "1", "0"))
            loConsulta.AppendLine(");")
            
        Next loRenglon

        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("--Actualiza los renglones")
        loConsulta.AppendLine("UPDATE  " & lcTablaRenglones )
        loConsulta.AppendLine("SET     Cod_Imp = #tblRenglones.Cod_Imp,")
        loConsulta.AppendLine("        Por_Imp1 = #tblRenglones.Por_Imp1,")
        loConsulta.AppendLine("        Mon_Imp1 = #tblRenglones.Mon_Imp1")
        loConsulta.AppendLine("FROM    #tblRenglones")
        loConsulta.AppendLine("WHERE   #tblRenglones.Renglon = " & lcTablaRenglones & ".Renglon")
        loConsulta.AppendLine("    AND " & lcTablaRenglones & ".Documento = @lcDocumento;")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("--Actualiza el encabezado: obtiene los tres impuestos principales")
        loConsulta.AppendLine("DECLARE @lcCod_Imp1 CHAR(10); SET @lcCod_Imp1 = '';")
        loConsulta.AppendLine("DECLARE @lcCod_Imp2 CHAR(10); SET @lcCod_Imp2 = '';")
        loConsulta.AppendLine("DECLARE @lcCod_Imp3 CHAR(10); SET @lcCod_Imp3 = '';")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DECLARE @lnPor_Imp1 DECIMAL(18,10); SET @lnPor_Imp1 = 0;")
        loConsulta.AppendLine("DECLARE @lnPor_Imp2 DECIMAL(18,10); SET @lnPor_Imp2 = 0;")
        loConsulta.AppendLine("DECLARE @lnPor_Imp3 DECIMAL(18,10); SET @lnPor_Imp3 = 0;")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SELECT  @lcCod_Imp1 = Cod_Imp,")
        loConsulta.AppendLine("        @lnPor_Imp1 = Por_Imp1")
        loConsulta.AppendLine("FROM    #tblRenglones")
        loConsulta.AppendLine("WHERE   #tblRenglones.Por_Imp1 = (SELECT MAX(Por_Imp1) FROM #tblRenglones);")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SELECT  @lcCod_Imp2 = Cod_Imp,")
        loConsulta.AppendLine("        @lnPor_Imp2 = Por_Imp1")
        loConsulta.AppendLine("FROM    #tblRenglones")
        loConsulta.AppendLine("WHERE   #tblRenglones.Por_Imp1 = (SELECT MAX(Por_Imp1) FROM #tblRenglones WHERE Cod_Imp NOT IN (@lcCod_Imp1));")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SELECT  @lcCod_Imp3 = Cod_Imp,")
        loConsulta.AppendLine("        @lnPor_Imp3 = Por_Imp1")
        loConsulta.AppendLine("FROM    #tblRenglones")
        loConsulta.AppendLine("WHERE   #tblRenglones.Por_Imp1 = (SELECT MAX(Por_Imp1) FROM #tblRenglones WHERE Cod_Imp NOT IN (@lcCod_Imp1, @lcCod_Imp2));")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("--Actualiza el encabezado: Distribución de impuestos")
        loConsulta.AppendLine("DECLARE @lcDistribucion AS VARCHAR(MAX);")
        loConsulta.AppendLine("SET @lcDistribucion = ( SELECT  RTRIM(Cod_Imp)                                          [codigo], ")
        loConsulta.AppendLine("                                CAST(Por_Imp1 AS " & lcDecimal28_X & ")                        [porcentaje], ")
        loConsulta.AppendLine("                                CAST((CASE WHEN Por_Imp1 > 0 ")
        loConsulta.AppendLine("                                    THEN SUM(Mon_Net) ELSE 0 END) AS " & lcDecimal28_X & ")    [base], ")
        loConsulta.AppendLine("                                CAST((CASE WHEN Por_Imp1 > 0 ")
        loConsulta.AppendLine("                                    THEN 0 ELSE SUM(Mon_Net) END) AS " & lcDecimal28_X & ")    [exento], ")
        loConsulta.AppendLine("                                0                                                       [sustraendo], ")
        loConsulta.AppendLine("                                CAST(SUM(Mon_Imp1) AS " & lcDecimal28_X & ")                   [monto],")
        loConsulta.AppendLine("                                COUNT(Renglon)                                          [renglones],")
        loConsulta.AppendLine("                                0                                                       [otros1], ")
        loConsulta.AppendLine("                                0                                                       [otros2], ")
        loConsulta.AppendLine("                                0                                                       [otros3], ")
        loConsulta.AppendLine("                                0                                                       [otros4], ")
        loConsulta.AppendLine("                                0                                                       [otros5], ")
        loConsulta.AppendLine("                                0                                                       [logico1],")
        loConsulta.AppendLine("                                0                                                       [logico2],")
        loConsulta.AppendLine("                                0                                                       [logico3],")
        loConsulta.AppendLine("                                0                                                       [logico4]")
        loConsulta.AppendLine("                        FROM    " & lcTablaRenglones)
        loConsulta.AppendLine("                        WHERE   Documento = @lcDocumento")
        loConsulta.AppendLine("                        GROUP BY Cod_Imp, Por_Imp1")
        loConsulta.AppendLine("                        ORDER BY Por_Imp1 DESC, Cod_Imp ASC")
        loConsulta.AppendLine("                        FOR XML PATH('impuesto'), ROOT('impuestos')")
        loConsulta.AppendLine("                    );")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("--Actualiza el encabezado: ")
        loConsulta.AppendLine("UPDATE  " & lcTablaEncabezado & "")
        loConsulta.AppendLine("SET     Mon_Imp1 = " & lnTotalImpuesto & ",")
        loConsulta.AppendLine("        Cod_Imp1 = @lcCod_Imp1,")
        loConsulta.AppendLine("        Cod_Imp2 = @lcCod_Imp2,")
        loConsulta.AppendLine("        Cod_Imp3 = @lcCod_Imp3,")
        loConsulta.AppendLine("        Por_Imp1 = @lnPor_Imp1,")
        loConsulta.AppendLine("        Por_Imp2 = @lnPor_Imp2,")
        loConsulta.AppendLine("        Por_Imp3 = @lnPor_Imp3,")
        loConsulta.AppendLine("        Mon_Net  = Mon_Bru + Mon_Rec1 - Mon_Des1 + Mon_Otr1 + Mon_Otr2 + Mon_Otr3 + " & lnTotalImpuesto & ",")
        If lcTabla.ToLower() = "compras" Then
            loConsulta.AppendLine("        Mon_Sal  = Mon_Bru + Mon_Rec1 - Mon_Des1 + Mon_Otr1 + Mon_Otr2 + Mon_Otr3 + " & lnTotalImpuesto & ",")
        End If
        loConsulta.AppendLine("        Dis_Imp  = @lcDistribucion")
        loConsulta.AppendLine("FROM    #tblRenglones")
        loConsulta.AppendLine("WHERE   " & lcTablaEncabezado & ".Documento = @lcDocumento;")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("-- Auditorias")
        loConsulta.AppendLine("DECLARE @lcDetalle AS VARCHAR(MAX);")
        loConsulta.AppendLine("SET @lcDetalle = COALESCE((")
        loConsulta.AppendLine("    SELECT  '<campos>' +")
        loConsulta.AppendLine("            (CASE WHEN Cod_Imp1_Antes <> Cod_Imp1_Despues ")
        loConsulta.AppendLine("                THEN '<campo nombre=""cod_imp1""><antes>' + Cod_Imp1_Antes")
        loConsulta.AppendLine("                    + '</antes><despues>' + Cod_Imp1_Despues + '</despues></campo>'")
        loConsulta.AppendLine("                ELSE '' END) +")
        loConsulta.AppendLine("            (CASE WHEN Cod_Imp2_Antes <> Cod_Imp2_Despues ")
        loConsulta.AppendLine("                THEN '<campo nombre=""cod_imp2""><antes>' + Cod_Imp2_Antes")
        loConsulta.AppendLine("                    + '</antes><despues>' + Cod_Imp2_Despues + '</despues></campo>'")
        loConsulta.AppendLine("                ELSE '' END) +")
        loConsulta.AppendLine("            (CASE WHEN Cod_Imp3_Antes <> Cod_Imp3_Despues ")
        loConsulta.AppendLine("                THEN '<campo nombre=""cod_imp3""><antes>' + Cod_Imp3_Antes")
        loConsulta.AppendLine("                    + '</antes><despues>' + Cod_Imp3_Despues + '</despues></campo>'")
        loConsulta.AppendLine("                ELSE '' END) +")
        loConsulta.AppendLine("            (CASE WHEN Por_Imp1_Antes <> Por_Imp1_Despues ")
        loConsulta.AppendLine("                THEN '<campo nombre=""por_imp1""><antes>' + Por_Imp1_Antes")
        loConsulta.AppendLine("                    + '</antes><despues>' + Por_Imp1_Despues + '</despues></campo>'")
        loConsulta.AppendLine("                ELSE '' END) +")
        loConsulta.AppendLine("            (CASE WHEN Por_Imp2_Antes <> Por_Imp2_Despues ")
        loConsulta.AppendLine("                THEN '<campo nombre=""pod_imp2""><antes>' + Por_Imp2_Antes")
        loConsulta.AppendLine("                    + '</antes><despues>' + Por_Imp2_Despues + '</despues></campo>'")
        loConsulta.AppendLine("                ELSE '' END) +")
        loConsulta.AppendLine("            (CASE WHEN Por_Imp3_Antes <> Por_Imp3_Despues ")
        loConsulta.AppendLine("                THEN '<campo nombre=""pod_imp3""><antes>' + Por_Imp3_Antes")
        loConsulta.AppendLine("                    + '</antes><despues>' + Por_Imp3_Despues + '</despues></campo>'")
        loConsulta.AppendLine("                ELSE '' END)  +")
        loConsulta.AppendLine("            (CASE WHEN Mon_Imp1_Antes <> Mon_Imp1_Despues ")
        loConsulta.AppendLine("                THEN '<campo nombre=""mon_imp1""><antes>' + Mon_Imp1_Antes")
        loConsulta.AppendLine("                    + '</antes><despues>' + Mon_Imp1_Despues + '</despues></campo>'")
        loConsulta.AppendLine("                ELSE '' END)  +")
        loConsulta.AppendLine("            (CASE WHEN Mon_Net_Antes <> Mon_Net_Despues ")
        loConsulta.AppendLine("                THEN '<campo nombre=""mon_net""><antes>' + Mon_Net_Antes")
        loConsulta.AppendLine("                    + '</antes><despues>' + Mon_Net_Despues + '</despues></campo>'")
        loConsulta.AppendLine("                ELSE '' END)  +")

        If lcTabla.ToLower() = "facturas" Then 

            loConsulta.AppendLine("            (CASE WHEN Mon_Net_Antes <> Mon_Net_Despues ")
            loConsulta.AppendLine("                THEN '<campo nombre=""mon_sal""><antes>' + Mon_Net_Antes")
            loConsulta.AppendLine("                    + '</antes><despues>' + Mon_Net_Despues + '</despues></campo>'")
            loConsulta.AppendLine("                ELSE '' END)   +")

        End If

        loConsulta.AppendLine("            (CASE WHEN Dis_Imp_Antes <> Dis_Imp_Despues ")
        loConsulta.AppendLine("                THEN '<campo nombre=""dis_imp""><antes>' + Dis_Imp_Antes")
        loConsulta.AppendLine("                    + '</antes><despues>' + Dis_Imp_Despues + '</despues></campo>'")
        loConsulta.AppendLine("                ELSE '' END) ")
        loConsulta.AppendLine("            + '</campos>'")
        loConsulta.AppendLine("    FROM (  SELECT  RTRIM(#tblEncabezadoOri.Cod_Imp1)                                       AS Cod_Imp1_Antes,")
        loConsulta.AppendLine("                    RTRIM(#tblEncabezadoOri.Cod_Imp2)                                       AS Cod_Imp2_Antes,")
        loConsulta.AppendLine("                    RTRIM(#tblEncabezadoOri.Cod_Imp3)                                       AS Cod_Imp3_Antes,")
        loConsulta.AppendLine("                    CAST(CAST(#tblEncabezadoOri.Por_Imp1 AS " & lcDecimal28_X & ") AS VARCHAR(50)) AS Por_Imp1_Antes,")
        loConsulta.AppendLine("                    CAST(CAST(#tblEncabezadoOri.Por_Imp2 AS " & lcDecimal28_X & ") AS VARCHAR(50)) AS Por_Imp2_Antes,")
        loConsulta.AppendLine("                    CAST(CAST(#tblEncabezadoOri.Por_Imp3 AS " & lcDecimal28_X & ") AS VARCHAR(50)) AS Por_Imp3_Antes,")
        loConsulta.AppendLine("                    CAST(CAST(#tblEncabezadoOri.Mon_Imp1 AS " & lcDecimal28_X & ") AS VARCHAR(50)) AS Mon_Imp1_Antes,")
        loConsulta.AppendLine("                    CAST(CAST(#tblEncabezadoOri.Mon_Net  AS " & lcDecimal28_X & ") AS VARCHAR(50)) AS Mon_Net_Antes,")
        loConsulta.AppendLine("                    RTRIM(#tblEncabezadoOri.Dis_Imp)                                            AS Dis_Imp_Antes,")
        loConsulta.AppendLine("                    RTRIM(Encabezado.Cod_Imp1)                                                  AS Cod_Imp1_Despues,")
        loConsulta.AppendLine("                    RTRIM(Encabezado.Cod_Imp2)                                                  AS Cod_Imp2_Despues,")
        loConsulta.AppendLine("                    RTRIM(Encabezado.Cod_Imp3)                                                  AS Cod_Imp3_Despues,")
        loConsulta.AppendLine("                    CAST(CAST(Encabezado.Por_Imp1  AS " & lcDecimal28_X & ") AS VARCHAR(50))      AS Por_Imp1_Despues,")
        loConsulta.AppendLine("                    CAST(CAST(Encabezado.Por_Imp2  AS " & lcDecimal28_X & ") AS VARCHAR(50))      AS Por_Imp2_Despues,")
        loConsulta.AppendLine("                    CAST(CAST(Encabezado.Por_Imp3  AS " & lcDecimal28_X & ") AS VARCHAR(50))      AS Por_Imp3_Despues,")
        loConsulta.AppendLine("                    CAST(CAST(Encabezado.Mon_Imp1  AS " & lcDecimal28_X & ") AS VARCHAR(50))      AS Mon_Imp1_Despues,")
        loConsulta.AppendLine("                    CAST(CAST(Encabezado.Mon_Net   AS " & lcDecimal28_X & ") AS VARCHAR(50))      AS Mon_Net_Despues,")
        loConsulta.AppendLine("                    RTRIM(Encabezado.Dis_Imp)                                                 AS Dis_Imp_Despues")
        loConsulta.AppendLine("            FROM    #tblEncabezadoOri")
        loConsulta.AppendLine("                JOIN " & lcTablaEncabezado & " Encabezado ON Encabezado.Documento = @lcDocumento")
        loConsulta.AppendLine("    ) X")
        loConsulta.AppendLine("), '');")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SET @lcDetalle = @lcDetalle + COALESCE((")
        loConsulta.AppendLine("    SELECT  '" & lcTablaRenglones.Replace("[", "").Replace("]", "") & "'    [@tabla],")
        loConsulta.AppendLine("            Renglon                 [@renglon],")
        loConsulta.AppendLine("            'modificado'            [@accion],")
        loConsulta.AppendLine("            cast(")
        loConsulta.AppendLine("            (CASE WHEN Cod_Imp_Antes <> Cod_Imp_Despues ")
        loConsulta.AppendLine("                THEN '<campo nombre=""cod_imp""><antes>' + Cod_Imp_Antes")
        loConsulta.AppendLine("                    + '</antes><despues>' + Cod_Imp_Despues + '</despues></campo>'")
        loConsulta.AppendLine("                ELSE '' END) +")
        loConsulta.AppendLine("            (CASE WHEN Por_Imp1_Antes <> Por_Imp1_Despues ")
        loConsulta.AppendLine("                THEN '<campo nombre=""por_imp1""><antes>' + Por_Imp1_Antes")
        loConsulta.AppendLine("                    + '</antes><despues>' + Por_Imp1_Despues + '</despues></campo>'")
        loConsulta.AppendLine("                ELSE '' END) +")
        loConsulta.AppendLine("            (CASE WHEN Mon_Imp1_Antes <> Mon_Imp1_Despues ")
        loConsulta.AppendLine("                THEN '<campo nombre=""mon_imp1""><antes>' + Mon_Imp1_Antes")
        loConsulta.AppendLine("                    + '</antes><despues>' + Mon_Imp1_Despues + '</despues></campo>'")
        loConsulta.AppendLine("                ELSE '' END) AS XML) ")
        loConsulta.AppendLine("    FROM (  SELECT  RTRIM(#tblRenglonesOri.Cod_Imp)                                          AS Cod_Imp_Antes,")
        loConsulta.AppendLine("                    CAST(CAST(#tblRenglonesOri.Por_Imp1 AS " & lcDecimal28_X & ") AS VARCHAR(50))   AS Por_Imp1_Antes,")
        loConsulta.AppendLine("                    CAST(CAST(#tblRenglonesOri.Mon_Imp1 AS " & lcDecimal28_X & ") AS VARCHAR(50))   AS Mon_Imp1_Antes,")
        loConsulta.AppendLine("                    RTRIM(Renglones.Cod_Imp)                                                 AS Cod_Imp_Despues,")
        loConsulta.AppendLine("                    CAST(CAST(Renglones.Por_Imp1 AS " & lcDecimal28_X & ") AS VARCHAR(50))          AS Por_Imp1_Despues,")
        loConsulta.AppendLine("                    CAST(CAST(Renglones.Mon_Imp1 AS " & lcDecimal28_X & ") AS VARCHAR(50))          AS Mon_Imp1_Despues,")
        loConsulta.AppendLine("                    #tblRenglonesOri.Renglon                                                 AS Renglon")
        loConsulta.AppendLine("            FROM    #tblRenglonesOri")
        loConsulta.AppendLine("                JOIN " & lcTablaRenglones & " Renglones")
        loConsulta.AppendLine("                ON Renglones.Documento = @lcDocumento")
        loConsulta.AppendLine("                AND Renglones.Renglon = #tblRenglonesOri.Renglon")
        loConsulta.AppendLine("    ) X")
        loConsulta.AppendLine("    WHERE  Cod_Imp_Antes <> Cod_Imp_Despues")
        loConsulta.AppendLine("        OR Por_Imp1_Antes <> Por_Imp1_Despues")
        loConsulta.AppendLine("        OR Mon_Imp1_Antes <> Mon_Imp1_Despues")
        loConsulta.AppendLine("    FOR XML PATH('renglon')")
        loConsulta.AppendLine("), '');")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("SET @lcDetalle = '<detalle>' + @lcDetalle + '</detalle>';")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")

		Dim lcTipoAuditoria			As String 
		Dim lcNombreOpcion			As String 
		Dim lcAccion				As String 
		Dim lcCodigo				As String 
		'Dim lcDocumento				As String 
		Dim lcDetalles				As String 
		Dim lcNombreEquipo			As String 
		Dim lcCodigoObjeto			As String 
		Dim lcNotasRegistro			As String 
		
		
		lcTipoAuditoria	 = "'Datos'"
		lcNombreOpcion	 = goServicios.mObtenerCampoFormatoSQL(Me.pcNombreOpcion)
		lcTabla		     = goServicios.mObtenerCampoFormatoSQL(lcTabla)
		lcAccion		 = "'Modificar'"
		lcCodigo		 = "'Sin código'"
		lcDocumento		 = goServicios.mObtenerCampoFormatoSQL(lcDocumento)
		lcDetalles		 = "@lcDetalle"
		lcNombreEquipo	 = goServicios.mObtenerCampoFormatoSQL(goAuditoria.pcNombreEquipo())
		lcCodigoObjeto	 = goServicios.mObtenerCampoFormatoSQL(TypeName(Me))
		lcNotasRegistro	 = "'Registro modificado: Cambio de impuesto desde el complementos Ajustar Impuestos.'"
			
		loConsulta.AppendLine(goAuditoria.mObtenerCadenaGuardar(lcTipoAuditoria,	_
																lcTabla,		_
																lcNombreOpcion,		_
																lcAccion,			_
																lcDocumento,		_
																lcCodigo,			_
																lcDetalles,			_
																lcNombreEquipo,		_
																lcCodigoObjeto,		_
																lcNotasRegistro))		

        loConsulta.AppendLine("")
        loConsulta.AppendLine("")

        Try
            Dim loDatos As New goDatos()

            loDatos.mEjecutarTransaccion(New String(){loConsulta.ToString()})

        Catch ex As Exception
            
            Me.wbcAdministradorMensajeModal.mMostrarMensajeModal("Operación no Completada", _
                                                                 "No fue posible cambiar el impuesto del documento. Información Adicional:<br/>" & _
                                                                 ex.Message, KN_Error, "600px", "500px")
            Return
        End Try

        Me.mBloquearFormulario()

        Me.wbcAdministradorMensajeModal.mMostrarMensajeModal("Operación Completada", _
                                                            "El impuesto del documento fue cambiado satisfactoriamente.", KN_Informacion)
        
        Try

            Dim lcConsulta As String = "SELECT TOP 1 * FROM " & lcTablaEncabezado & " WHERE " & lcCondicionSalida
            Dim loTabla As DataTable = (New goDatos()).mObtenerTodosSinEsquema(lcConsulta, lcTabla).Tables(0)
            If loTabla.Rows.Count > 0 Then 
                goBusquedaRegistro.poRegistroSeleccionado = loTabla.Rows(0)
            End If  

            Me.WbcAdministradorVentanaModal.mMostrarVentanaModal(lcFormularioSalida, "740px", "480px", False, True)

        Catch ex As Exception
            
        End Try

    End Sub

#End Region

#Region "Eventos"
	
	Protected Sub cmdAceptar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAceptar.Click

        Me.mGuardarCambios()

	End Sub
	 
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not Me.IsPostBack()

			Me.pnDecimalesMonto		    = goOpciones.pnDecimalesParaMonto
			Me.pnDecimalesCantidad		= goOpciones.pnDecimalesParaCantidad
			Me.pnTasaMonedaAdicional    = goMoneda.pnTasaMonedaAdicional

            'Leer los parámetros enviados al complemento
            Dim laParametros As Generic.Dictionary(Of String, Object)
            laParametros = Me.Session("frmComplementos.paParametros")
            Me.Session.Remove("frmComplementos.paParametros")
		
            Dim lcDocumento As String = ""
            Dim lcTabla As String = ""

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

                lcDocumento = CStr(laIndices("Documento"))
                lcTabla = CStr(laParametros("lcTabla"))

                Me.lblTitulo.Text = "Ajustar Impuestos: " & lcDocumento & " (" & CStr(laParametros("lcNombreOpcion")) & ")"

                Me.paParametros = laParametros

            End If

            If String.IsNullOrEmpty(lcTabla) Then 
                Me.mBloquearFormulario()
            Else 
                Me.mCargarImpuestos(lcDocumento, lcTabla)
            End If
            
        End If

    End Sub


    Protected Sub cboImpuestoAnterior_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboImpuestoAnterior.SelectedIndexChanged

        Me.mCargarDatos(Me.poRenglonesOriginales)
         
    End Sub

    Protected Sub cboImpuestoNuevo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboImpuestoNuevo.SelectedIndexChanged

        Me.mCargarDatos(Me.poRenglonesOriginales)

    End Sub

#End Region


End Class
'-------------------------------------------------------------------------------------------'
' Fin del codigo																			'
'-------------------------------------------------------------------------------------------'
' RJG: 05/01/17: Codigo Inicial																'
'-------------------------------------------------------------------------------------------'
' RJG: 07/01/17: Se agregó la apertura del documento modificado.                            '
'-------------------------------------------------------------------------------------------'
