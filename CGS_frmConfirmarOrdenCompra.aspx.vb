'-------------------------------------------------------------------------------------------'
' Inicio del codigo
'-------------------------------------------------------------------------------------------'
Imports vis3Controles.wbcAdministradorMensajeModal
Imports Microsoft.VisualBasic

'-------------------------------------------------------------------------------------------'
' Inicio de clase "CGS_frmConfirmarOrdenCompra"
'-------------------------------------------------------------------------------------------'
Partial Class CGS_frmConfirmarOrdenCompra
	Inherits vis2formularios.frmFormularioGenerico

#Region "Declaraciones"

#End Region

#Region "Propiedades"
    Private Property paParametros As Generic.Dictionary(Of String, Object)
        Get
            Return Me.ViewState("paParametros")
        End Get
        Set(value As Generic.Dictionary(Of String, Object))
            Me.ViewState("paParametros") = value
        End Set
    End Property
#End Region

#Region "Eventos"

    Protected Sub mCargaPagina(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		
		'La primera vez que se cargue el formulario...
		If Not Me.IsPostBack() Then


            Me.txtDocumento.mConfigurarBusqueda("Ordenes_Compras", _
                                              "Documento", _
                                              "Documento,Comentario,status", _
                                              ".,Documento,Comentario,Estatus", _
                                              "Documento,Comentario,status", _
                                              "../../Framework/Formularios/frmFormularioBusqueda.aspx", _
                                              "Documento", _
                                              "", "Status IN ('Pendiente')")


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

                Me.txtDocumento.pcTexto("Documento") = CStr(laIndices("Documento"))

                Me.paParametros = laParametros

                If Me.txtDocumento.mValidarEntrada("Documento") Then
                    Me.txtDocumento_mResultadoBusquedaValido(Me.txtDocumento.mObtenerControl("Documento"), "Documento", 0)
                Else
                    Me.txtDocumento.mLimpiarCampos()
                End If

            End If

        End If
			
	End Sub

	Protected Sub cmdAceptar_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
	Handles cmdAceptar.Click

        Dim lcNumero As String = goServicios.mObtenerCampoFormatoSQL(Strings.Trim(Me.txtDocumento.pcTexto("Documento")))
        Dim lcUsuario As String = goUsuario.pcCodigo
        Dim ldFecha As String = goServicios.mObtenerCampoFormatoSQL(Date.Now())

        Dim loDatos As New goDatos()
        Dim loSentencias As New StringBuilder()
        Dim loTransacccion As New ArrayList()

        'COLOCAR MARCA DE CONFIRMACIÓN SEGÚN USUARIO
        'CADA USUARIO TIENE ASIGANADO UN CAMPO LOGICO EN LA TABLA ordenes_compras QUE INDICA QUE LA ORDEN DE COMPRA FUE CONFIRMADA POR ÉL
        If (lcUsuario = "mgentili") Then
            loSentencias.AppendLine("UPDATE Ordenes_Compras SET Logico1 = 1, Fecha1 = " & ldFecha & " WHERE Documento = " & lcNumero)
        ElseIf (lcUsuario = "ssimanca") Then
            loSentencias.AppendLine("UPDATE Ordenes_Compras SET Logico2 = 1, Fecha2 = " & ldFecha & " WHERE Documento = " & lcNumero)
        ElseIf (lcUsuario = "lcarrizal") Then
            loSentencias.AppendLine("UPDATE Ordenes_Compras SET Logico3 = 1, Fecha3 = " & ldFecha & " WHERE Documento = " & lcNumero)
        ElseIf (lcUsuario = "yreina") Then
            loSentencias.AppendLine("UPDATE Ordenes_Compras SET Logico4 = 1, Fecha4 = " & ldFecha & " WHERE Documento = " & lcNumero)
        ElseIf (lcUsuario = "kodeitsu") Then
            loSentencias.AppendLine("UPDATE Ordenes_Compras SET Logico5 = 1, Fecha5 = " & ldFecha & " WHERE Documento = " & lcNumero)
        End If

        loTransacccion.Add(loSentencias.ToString())

        Try
            loDatos.mEjecutarTransaccion(loTransacccion)

        Catch loExcepcion As Exception
            Me.mMostrarMensajeModal("Operación no Completada", _
                    "No fue posible completar la confirmación del documento. <br/>Información Adicional:" & _
                    loExcepcion.Message, "e")
            Return
        End Try

        loSentencias.Length = 0

        'VERIFICAR CANTIDAD DE CONFIRMACIONES QUE LLEVA EL DOCUMENTO
        loSentencias.AppendLine("SELECT logico1 AS mgentili, logico2 AS ssimanca, logico3 AS lcarrizal, logico4 AS yreina,logico5 AS kodeitsu")
        loSentencias.AppendLine("FROM Ordenes_Compras")
        loSentencias.AppendLine("WHERE Documento = " & lcNumero)
        loSentencias.AppendLine("")

        Dim loTabla As DataTable

        loTabla = loDatos.mObtenerTodosSinEsquema(loSentencias.ToString(), "Ordenes_Compras").Tables(0)

        Dim lcConfirmado(5) As Boolean
        lcConfirmado(0) = loTabla.Rows(0).Item("mgentili")
        lcConfirmado(1) = loTabla.Rows(0).Item("ssimanca")
        lcConfirmado(2) = loTabla.Rows(0).Item("lcarrizal")
        lcConfirmado(3) = loTabla.Rows(0).Item("yreina")
        lcConfirmado(4) = loTabla.Rows(0).Item("kodeitsu")

        Dim lnCount As Integer = 0D
        For index As Integer = 0 To lcConfirmado.GetUpperBound(0)
            If lcConfirmado(index) = True Then
                lnCount += 1
            End If
        Next

        loSentencias.Length = 0

        If lnCount = 1 Then 'SI EL DOCUMENTO HA SIDO CONFIRMADO UNA SOLA VEZ SE NOTIFICA SU CONFIRMACIÓN EXITOSA
            Me.mMostrarMensajeModal("Operación Completada", "El Documento fue confirmado satisfactoriamente. ", "i")
        ElseIf lnCount = 2 Then 'SI EL DOCUMENTO SE CONFIRMÓ DOS VECES

            'EVENTO "Antes de Confirmar"
            '-------------------------------------------------------------------------------------------
            ' Dispara los eventos "Antes de Confirmar" y "Despues de Confirmar".
            '-------------------------------------------------------------------------------------------
            Dim llEventosActivos As Boolean = CBool(cusAplicacion.goOpciones.mObtener("ACTEVE", "L")) 
            Dim laVista As DataRow = Nothing 
            Dim laRenglones() As DataTable = Nothing '= New DataTable(){ loTablaDatos }
            
            If llEventosActivos Then 

                Dim loConsulta As New StringBuilder()
                'Agregar todos los campos necesarios del encabezado y los renglones
                loConsulta.AppendLine("")
                loConsulta.AppendLine("SELECT  Documento, Control, Fec_Ini, Fec_Fin, Mon_Net,Usu_Cre")
                loConsulta.AppendLine("FROM    Ordenes_Compras")
                loConsulta.AppendLine("WHERE   Documento = " & lcNumero)
                loConsulta.AppendLine("")
                loConsulta.AppendLine("SELECT  Documento, Renglon, Cod_Art, Can_Art1, Precio1, Mon_Net")
                loConsulta.AppendLine("FROM    Renglones_oCompras")
                loConsulta.AppendLine("WHERE   Documento = " & lcNumero)
                loConsulta.AppendLine("ORDER BY Renglon ASC")
                loConsulta.AppendLine("")
                loConsulta.AppendLine("")

                Dim loCompra As DataSet
            
                loCompra = (New goDatos()).mObtenerTodosSinEsquema(loConsulta.ToString(), "Ordenes_Compras")


                'El encabezado se pasa como un soloDataRow
                laVista = loCompra.Tables(0).Rows(0)
                'Los renglones se toman directamente como un array de DataTable
                laRenglones = New DataTable(){ loCompra.Tables(1) }

                '-------------------------------------------------------------------------------------------
                ' Ejecutar el gancho "ANTES_CONFIRMAR"
                '-------------------------------------------------------------------------------------------
                'Gancho Básico (para eventos de formularios registrados)
                'Dim loResultado = goEvento.mGanchoGenerico("OrdenesCompra", "ANTES_CONFIRMAR", laVista, laRenglones, False)
                'Gancho Genérico (para eventos de pantalla personalizada)
                Dim loResultado = goEvento.mGancho("OrdenesCompra", "ANTES_CONFIRMAR", laVista, laRenglones, New goEntornoFramework(True), False)

                'Si la operación se cancela (e.g. si el evento responde "no continuar")
                'No se guarda el registro actual (el "Aceptar" no se ejecuta)
                If Not loResultado.plContinuarEjecucion Then

                    Me.mMostrarMensajeModal(loResultado.pcTituloMensaje, loResultado.pcContenidoMensaje, "a")

                    Return 
                End If

            End If

            'FIN EVENTO "Antes de Confirmar"



            Dim lcConsulta As New StringBuilder()
            Dim loRenglonesDatos As New goDatos()

            'TRAER RENGLONES DE LA ORDEN DE COMPRA PARA VERIFICAR SI EL DOCUMENTO TIENE ASOCIADO REQUISICIÓN INTERNA
            lcConsulta.AppendLine("SELECT   Renglones_OCompras.Renglon,")
            lcConsulta.AppendLine("         Renglones_OCompras.Cod_Art,")
            lcConsulta.AppendLine("         Renglones_OCompras.Cod_Alm,")
            lcConsulta.AppendLine("         Renglones_OCompras.Can_Art1,")
            lcConsulta.AppendLine("         Renglones_OCompras.Tip_Ori,")
            lcConsulta.AppendLine("         Renglones_OCompras.Doc_Ori,")
            lcConsulta.AppendLine("         Renglones_OCompras.Ren_Ori,")
            lcConsulta.AppendLine("         Articulos.Tipo")
            lcConsulta.AppendLine("FROM Renglones_OCompras")
            lcConsulta.AppendLine(" JOIN Articulos ON Articulos.Cod_Art = Renglones_OCompras.Cod_Art")
            lcConsulta.AppendLine("WHERE Documento = " & lcNumero)
            lcConsulta.AppendLine("")
            lcConsulta.AppendLine("")
            lcConsulta.AppendLine("")
            lcConsulta.AppendLine("")
            lcConsulta.AppendLine("")

            Dim loTablaRenglones As DataSet

            loTablaRenglones = loRenglonesDatos.mObtenerTodosSinEsquema(lcConsulta.ToString(), "Renglones_OCompras")

            For lnNumeroFila As Integer = 0 To loTablaRenglones.Tables(0).Rows.Count - 1
                Dim ldCantidad As String = goServicios.mObtenerCampoFormatoSQL(CDec(loTablaRenglones.Tables(0).Rows(lnNumeroFila).Item("Can_Art1")))
                Dim lcDocOri As String = goServicios.mObtenerCampoFormatoSQL(CStr(loTablaRenglones.Tables(0).Rows(lnNumeroFila).Item("Doc_Ori")).Trim())
                Dim lcRenOri As String = goServicios.mObtenerCampoFormatoSQL(CStr(loTablaRenglones.Tables(0).Rows(lnNumeroFila).Item("Ren_Ori")).Trim())

                If CStr(loTablaRenglones.Tables(0).Rows(lnNumeroFila).Item("Doc_Ori")).Trim() <> "" Then
                    'REBAJAR CANTIDADES PENDIENTES DE REQUISICIÓN Y CAMBIAR SU ESTATUS
                    loSentencias.AppendLine("UPDATE Renglones_Requisiciones SET Can_Pen1 = Can_Pen1 - " & ldCantidad)
                    loSentencias.AppendLine("WHERE Documento = " & lcDocOri & " AND Renglon = " & lcRenOri)
                    loSentencias.AppendLine("")
                    loSentencias.AppendLine("IF (SELECT SUM(Can_pen1) FROM Renglones_Requisiciones WHERE Documento = " & lcDocOri & ") = 0")
                    loSentencias.AppendLine("BEGIN	")
                    loSentencias.AppendLine("	UPDATE Requisiciones SET Status = 'Procesado' WHERE Documento = " & lcDocOri)
                    loSentencias.AppendLine("END")
                    loSentencias.AppendLine("ELSE")
                    loSentencias.AppendLine("BEGIN	")
                    loSentencias.AppendLine("	UPDATE Requisiciones SET Status = 'Afectado' WHERE Documento = " & lcDocOri)
                    loSentencias.AppendLine("END")
                    loSentencias.AppendLine("")
                End If

                If CStr(loTablaRenglones.Tables(0).Rows(lnNumeroFila).Item("Tipo")).Trim() <> "Servicio" Then
                    'REBAJAR CANTIDADES POR LLEGAR EN ARTÍCULO Y ALMACÉN
                    Dim lcCodArt As String = goServicios.mObtenerCampoFormatoSQL(CStr(loTablaRenglones.Tables(0).Rows(lnNumeroFila).Item("Cod_Art")).Trim())
                    Dim lcCodAlm As String = goServicios.mObtenerCampoFormatoSQL(CStr(loTablaRenglones.Tables(0).Rows(lnNumeroFila).Item("Cod_Alm")).Trim())

                    loSentencias.AppendLine("UPDATE Articulos SET Exi_Por1 = Exi_Por1 + " & ldCantidad)
                    loSentencias.AppendLine("WHERE Cod_Art = " & lcCodArt)
                    loSentencias.AppendLine("")
                    loSentencias.AppendLine("IF EXISTS((SELECT * FROM Renglones_Almacenes WHERE Cod_Alm = " & lcCodAlm & " AND Cod_Art = " & lcCodArt & "))")
                    loSentencias.AppendLine("BEGIN")
                    loSentencias.AppendLine("   UPDATE Renglones_Almacenes SET Exi_Por1 = Exi_Por1 + " & ldCantidad)
                    loSentencias.AppendLine("   WHERE Cod_Alm = " & lcCodAlm & " AND Cod_Art = " & lcCodArt)
                    loSentencias.AppendLine("END")
                    loSentencias.AppendLine("ELSE")
                    loSentencias.AppendLine("BEGIN")
                    loSentencias.AppendLine("	INSERT INTO Renglones_Almacenes (Cod_Alm,Cod_Art,Exi_Por1) VALUES (" & lcCodAlm & "," & lcCodArt & "," & ldCantidad & ")")
                    loSentencias.AppendLine("END")
                    loSentencias.AppendLine("")
                    loSentencias.AppendLine("")
                    loSentencias.AppendLine("")
                End If
            Next lnNumeroFila

            'CAMBIAR ESTATUS DE LA ORDEN DE COMPRA A "CONFIRMADO"
            loSentencias.AppendLine("UPDATE Ordenes_Compras SET Status = 'Confirmado' WHERE Documento = " & lcNumero)
            loSentencias.AppendLine("")
            loSentencias.AppendLine("")
            loSentencias.AppendLine("")

            loTransacccion.Add(loSentencias.ToString())

            Try
                loDatos.mEjecutarTransaccion(loTransacccion)

                Me.mMostrarMensajeModal("Operación Completada", "El Documento fue confirmado satisfactoriamente. ", "i")
            Catch loExcepcion As Exception
                'Un error al ejecutar la transaccion principal lo mostramos como mensaje tipo "Error"
                Me.mMostrarMensajeModal("Operación no Completada", _
                    "No fue posible completar la confirmación del documento. <br/>Información Adicional:" & _
                    loExcepcion.Message, "e")
            End Try


            'INICIO EVENTO "Después de Confirmar"
            If llEventosActivos Then

                '-------------------------------------------------------------------------------------------
                ' Ejecutar el gancho "DESPUES_CONFIRMAR"
                '-------------------------------------------------------------------------------------------
                'Gancho Básico (para eventos de formularios registrados)
                'Dim loResultado = goEvento.mGanchoGenerico("OrdenesCompra", "DESPUES_CONFIRMAR", laVista, laRenglones, False)
                'Gancho Genérico (para eventos de pantalla personalizada)
                Dim loResultado = goEvento.mGancho("OrdenesCompra", "DESPUES_CONFIRMAR", laVista, laRenglones, New goEntornoFramework(True), False)
            
                'Si la operación se cancela (e.g. si el evento responde "no continuar")
                'El registro ya se ha guardado (no se puede evitar), pero debe mostrarse el mensaje del evento
                If Not loResultado.plContinuarEjecucion Then

                    Me.mMostrarMensajeModal(loResultado.pcTituloMensaje, loResultado.pcContenidoMensaje, "a")

                    Return 

                End If

            End If
            'FIN EVENTO "Despues de Confirmar"


        End If

        'loSentencias.Length = 0


        '-------------------------------------------------------------------------------------------'
        ' Prepara la auditoria.																		'
        '-------------------------------------------------------------------------------------------'
        Dim laCadenaTransacciones As New ArrayList
        Dim loEjecutarTransaccion As New cusDatos.goDatos

        Dim lcTipoAuditoria As String = "'Datos'"
        Dim lcTabla As String = "'Ordenes_Compras'"
        Dim lcNombreOpcion As String = goServicios.mObtenerCampoFormatoSQL(Me.pcNombreOpcion)
        Dim lcAccion As String = "'Confirmar'"
        Dim lcDocumento As String = lcNumero
        Dim lcCodigoRegistro As String = "'Sin código'"
        Dim lcDetalle As String = ""
        If lnCount = 2 Then
            lcDetalle = goServicios.mObtenerCampoFormatoSQL(goAuditoria.mGenerarCampoDetalle("status", "Pendiente", "Confirmado"))
        Else
            lcDetalle = goServicios.mObtenerCampoFormatoSQL(goAuditoria.KC_DetalleVacio)
        End If
        Dim lcNombreEquipo As String = goServicios.mObtenerCampoFormatoSQL(goAuditoria.pcNombreEquipo)
        Dim lcCodigoObjeto As String = goServicios.mObtenerCampoFormatoSQL(TypeName(Me))
        Dim lcNotas As String = "'Documento Confirmado desde complemento ""Confirmar Orden de Compra"".'"
        Dim lcClave2 As String = "''"
        Dim lcClave3 As String = "''"
        Dim lcClave4 As String = "''"
        Dim lcClave5 As String = "''"

        Dim lcInsercionAuditoria As String
        lcInsercionAuditoria = goAuditoria.mObtenerCadenaGuardar(lcTipoAuditoria, _
                                                                lcTabla, _
                                                                lcNombreOpcion, _
                                                                lcAccion, _
                                                                lcDocumento, _
                                                                lcCodigoRegistro, _
                                                                lcDetalle, _
                                                                lcNombreEquipo, _
                                                                lcCodigoObjeto, _
                                                                lcNotas, _
                                                                lcClave2, lcClave3, lcClave4, lcClave5)




        Try
            'INSERCIÓN DE AUDITORÍA
            loDatos.mEjecutarComando(lcInsercionAuditoria)

        Catch loExcepcion As Exception

            Me.mMostrarMensajeModal("Operación Completada", "El Documento fue confirmado satisfactoriamente, sin embargo no fue posible guardar el registro de auditoria. <br/>Información Adicional: " & loExcepcion.Message, "a")

        End Try

        If lnCount = 2 Then

            'El número de documento se toma del control en pantalla (por si acaso el usuario lo cambió manualmente).
            'Dim lcDoc As String = DirectCast(Me.paParametros("laIndices"), Generic.Dictionary(Of String, Object))("Documento")
            Dim lcDoc As String = Strings.Trim(Me.txtDocumento.pcTexto("Documento"))

            Dim lcTablaEncabezado As String = "[Ordenes_Compras]"
            Dim lcTablaRenglones As String = "[Renglones_OCompras]"
            Dim lcFormularioSalida As String = "../../Administrativo/Formularios/frmOperacionOrdenesCompra.aspx"
            Dim lcCondicionSalida As String = CStr(Me.paParametros("lcCondicion"))

            Try

                Dim lcConsulta As String = "SELECT TOP 1 * FROM " & lcTablaEncabezado & " WHERE " & lcCondicionSalida
                Dim loTablaDoc As DataTable = (New goDatos()).mObtenerTodosSinEsquema(lcConsulta, lcTabla).Tables(0)
                If loTablaDoc.Rows.Count > 0 Then
                    goBusquedaRegistro.poRegistroSeleccionado = loTablaDoc.Rows(0)
                End If

                Me.WbcAdministradorVentanaModal.mMostrarVentanaModal(lcFormularioSalida, "740px", "480px", False, True)

            Catch ex As Exception

            End Try
        End If

        Me.txtDocumento.mLimpiarCampos()
        Me.txtProveedor.Text = ""
        Me.TxtComentario.Text = ""
        Me.cmdAceptar.Enabled = False
        Me.cmdCancelar.Text = "Cerrar"

    End Sub

    Protected Sub txtDocumento_mResultadoBusquedaNoValido(ByVal sender As vis1Controles.txtNormal, ByVal lcNombreCampo As String, ByVal lnIndice As Integer) Handles txtDocumento.mResultadoBusquedaNoValido

        Me.mMostrarMensajeModal("Advertencia", _
            "El Número de Documento indicado no es válido.", "a")

        Me.txtDocumento.pcTexto("Documento") = ""

        Me.txtProveedor.Text = ""
        Me.TxtComentario.Text = ""

    End Sub

    Protected Sub txtDocumento_mResultadoBusquedaValido(ByVal sender As vis1Controles.txtNormal, ByVal lcNombreCampo As String, ByVal lnIndice As Integer) Handles txtDocumento.mResultadoBusquedaValido

        Dim lcNumero As String = Strings.Trim(Me.txtDocumento.pcTexto("Documento"))
        Dim lcUsuario As String = goUsuario.pcCodigo

        'VERIFICA QUE EL USUARIO TENGA PERMITIDO CONFIRMAR
        If (lcUsuario <> "mgentili" And lcUsuario <> "ssimanca" And lcUsuario <> "lcarrizal" And lcUsuario <> "yreina" And lcUsuario <> "kodeitsu") Then

            Me.mMostrarMensajeModal("Operación no Completada", "Usted no tiene permisos para confirmar la orden de compra. ", "a")
            Return
        End If

        Dim loTabla As DataTable
        Dim loDatos As New goDatos
        Dim loConsulta As New StringBuilder()

        loConsulta.AppendLine("SELECT Ordenes_Compras.Status,")
        loConsulta.AppendLine("       Ordenes_Compras.Cod_Pro,")
        loConsulta.AppendLine("       Ordenes_Compras.Comentario,")
        loConsulta.AppendLine("       Ordenes_Compras.Prioridad,")
        loConsulta.AppendLine("       Proveedores.Nom_Pro,")
        loConsulta.AppendLine("       Ordenes_Compras.logico1 AS mgentili,")
        loConsulta.AppendLine("       Ordenes_Compras.logico2 AS ssimanca,")
        loConsulta.AppendLine("       Ordenes_Compras.logico3 AS lcarrizal,")
        loConsulta.AppendLine("       Ordenes_Compras.logico4 AS yreina,")
        loConsulta.AppendLine("       Ordenes_Compras.logico5 AS kodeitsu")
        loConsulta.AppendLine("FROM Ordenes_Compras")
        loConsulta.AppendLine(" JOIN Proveedores ON Proveedores.Cod_Pro = Ordenes_Compras.Cod_Pro")
        loConsulta.AppendLine("WHERE Ordenes_Compras.Documento = " & goServicios.mObtenerCampoFormatoSQL(lcNumero))
        loConsulta.AppendLine("")

        loTabla = loDatos.mObtenerTodosSinEsquema(loConsulta.ToString(), "Ordenes_Compras").Tables(0)

        If CStr(loTabla.Rows(0).Item("Status")).Trim() <> "Pendiente" Then
            Me.mMostrarMensajeModal("Advertencia", "El Documento no puede ser confirmado.", "a")
            Me.cmdAceptar.Enabled = False
            Me.cmdCancelar.Text = "Cerrar"
            Return
        ElseIf CStr(loTabla.Rows(0).Item("Prioridad")).Trim() = "PDC" Then
            Me.mMostrarMensajeModal("Advertencia", "El Documento es de material de producción y no puede ser confirmado.", "a")
            Me.cmdAceptar.Enabled = False
            Me.cmdCancelar.Text = "Cerrar"
        ElseIf (CBool(loTabla.Rows(0).Item("mgentili")) = True And lcUsuario = "mgentili") Then
            Me.mMostrarMensajeModal("Advertencia", "El Documento ya fue confirmado por usted.", "a")
            Me.cmdAceptar.Enabled = False
            Me.cmdCancelar.Text = "Cerrar"
            Return
        ElseIf (CBool(loTabla.Rows(0).Item("ssimanca")) = True And lcUsuario = "ssimanca") Then
            Me.mMostrarMensajeModal("Advertencia", "El Documento ya fue confirmado por usted.", "a")
            Me.cmdAceptar.Enabled = False
            Me.cmdCancelar.Text = "Cerrar"
            Return
        ElseIf (CBool(loTabla.Rows(0).Item("lcarrizal")) = True And lcUsuario = "lcarrizal") Then
            Me.mMostrarMensajeModal("Advertencia", "El Documento ya fue confirmado por usted.", "a")
            Me.cmdAceptar.Enabled = False
            Me.cmdCancelar.Text = "Cerrar"
            Return
        ElseIf (CBool(loTabla.Rows(0).Item("yreina")) = True And lcUsuario = "yreina") Then
            Me.mMostrarMensajeModal("Advertencia", "El Documento ya fue confirmado por usted.", "a")
            Me.cmdAceptar.Enabled = False
            Me.cmdCancelar.Text = "Cerrar"
            Return
        ElseIf (CBool(loTabla.Rows(0).Item("kodeitsu")) = True And lcUsuario = "kodeitsu") Then
            Me.mMostrarMensajeModal("Advertencia", "El Documento ya fue confirmado por usted.", "a")
            Me.cmdAceptar.Enabled = False
            Me.cmdCancelar.Text = "Cerrar"
            Return
        Else
            Me.txtProveedor.Text = CStr(loTabla.Rows(0).Item("Cod_Pro")).Trim() & " - " & CStr(loTabla.Rows(0).Item("Nom_Pro")).Trim()
            Me.TxtComentario.Text = CStr(loTabla.Rows(0).Item("Comentario")).Trim()
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
	Private Sub mMostrarMensajeModal(lcTitulo As String, lcContenido As String, lcTipo As String)
		Dim loScript As New StringBuilder() 

		lcTipo = lcTipo.ToLower()
		
		If	(lcTipo <> "a") AndAlso _
			(lcTipo <> "e") AndAlso _
			(lcTipo <> "i") Then
			
			lcTipo = "a"
			
		End If
		
		
		loScript.Append("window.poMensajes.mMostrarMensajeModal('")
		loScript.Append(lcTitulo.Replace("'", "\'"))
		loScript.Append("','")
		loScript.Append(lcContenido.Replace("'", "\'"))
		loScript.Append("','")
		loScript.Append(lcTipo)
		loScript.AppendLine("', 250, 500);")
		
		ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Mensaje", loScript.toString(), True)
		
	End Sub
	
#End Region

	
End Class
'-------------------------------------------------------------------------------------------'
' Fin del codigo																			'
'-------------------------------------------------------------------------------------------'
' RJG: 22/09/17: Codigo Inicial																'
'-------------------------------------------------------------------------------------------'
' RJG: 17/01/18: Agregué los ganchos "ANTES_CONFIRMAR" y "DESPUES_CONFIRMAR".               '
'-------------------------------------------------------------------------------------------'
