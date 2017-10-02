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

        If (lcUsuario = "mgentili") Then
            loSentencias.AppendLine("UPDATE Ordenes_Compras SET Logico1 = 1, Fecha1 = " & ldFecha & " WHERE Documento = " & lcNumero)
        ElseIf (lcUsuario = "ssminanca") Then
            loSentencias.AppendLine("UPDATE Ordenes_Compras SET Logico2 = 1, Fecha2 = " & ldFecha & " WHERE Documento = " & lcNumero)
        ElseIf (lcUsuario = "dmatheus") Then
            loSentencias.AppendLine("UPDATE Ordenes_Compras SET Logico3 = 1, Fecha3 = " & ldFecha & " WHERE Documento = " & lcNumero)
        ElseIf (lcUsuario = "yreina") Then
            loSentencias.AppendLine("UPDATE Ordenes_Compras SET Logico4 = 1, Fecha4 = " & ldFecha & " WHERE Documento = " & lcNumero)
        ElseIf (lcUsuario = "kodeitsu") Then
            loSentencias.AppendLine("UPDATE Ordenes_Compras SET Logico5 = 1, Fecha5 = " & ldFecha & " WHERE Documento = " & lcNumero)
        End If

        loTransacccion.Add(loSentencias.ToString())

        Try
            loDatos.mEjecutarTransaccion(loTransacccion)
            'Muestra un mensaje tipo "Información" 
            Me.mMostrarMensajeModal("Operación Completada", "El Documento fue confirmado satisfactoriamente. ", "i")
        Catch loExcepcion As Exception
            'Un error al ejecutar la transaccion principal lo mostramos como mensaje tipo "Error"
            Me.mMostrarMensajeModal("Operación no Completada", _
                    "No fue posible completar la confirmación del documento. <br/>Información Adicional:" & _
                    loExcepcion.Message, "e")
        End Try

        loSentencias.Length = 0

        loSentencias.AppendLine("SELECT logico1 AS mgentili, logico2 AS ssimanca, logico3 AS dmatheus, logico4 AS yreina,logico5 AS kodeitsu")
        loSentencias.AppendLine("FROM Ordenes_Compras")
        loSentencias.AppendLine("WHERE Documento = " & lcNumero)
        loSentencias.AppendLine("")

        Dim loTabla As DataTable

        loTabla = loDatos.mObtenerTodosSinEsquema(loSentencias.ToString(), "Ordenes_Compras").Tables(0)

        Dim lcConfirmado(5) As Boolean
        lcConfirmado(0) = loTabla.Rows(0).Item("mgentili")
        lcConfirmado(1) = loTabla.Rows(0).Item("ssimanca")
        lcConfirmado(2) = loTabla.Rows(0).Item("dmatheus")
        lcConfirmado(3) = loTabla.Rows(0).Item("yreina")
        lcConfirmado(4) = loTabla.Rows(0).Item("kodeitsu")

        Dim count As Integer = 0D
        For index As Integer = 0 To lcConfirmado.GetUpperBound(0)
            If lcConfirmado(index) = True Then
                count += 1
            End If
        Next

        'If lcConfirmado(0) = True Then
        '    count += 1
        'ElseIf lcConfirmado(1) = True Then
        '    count += 1
        'ElseIf lcConfirmado(2) = True Then
        '    count += 1
        'ElseIf lcConfirmado(3) = True Then
        '    count += 1
        'ElseIf lcConfirmado(4) = True Then
        '    count += 1
        'ElseIf lcConfirmado(5) = True Then
        '    count += 1
        'End If

        loSentencias.Length = 0

        If count = 2 Then
            loSentencias.AppendLine("UPDATE Ordenes_Compras SET Status = 'Confirmado' WHERE Documento = " & lcNumero)

            loTransacccion.Add(loSentencias.ToString())
            Try
                loDatos.mEjecutarTransaccion(loTransacccion)
                'Muestra un mensaje tipo "Información" 
                Me.mMostrarMensajeModal("Operación Completada", "El Documento fue confirmado satisfactoriamente. ", "i")
            Catch loExcepcion As Exception
                'Un error al ejecutar la transaccion principal lo mostramos como mensaje tipo "Error"
                Me.mMostrarMensajeModal("Operación no Completada", _
                    "No fue posible completar la confirmación del documento. <br/>Información Adicional:" & _
                    loExcepcion.Message, "e")
            End Try
        End If

        'loSentencias.Length = 0


        '-------------------------------------------------------------------------------------------'
        ' Prepara la auditoria.																		'
        '-------------------------------------------------------------------------------------------'
        'Dim lcTipoAuditoria		As String	= "'Datos'"
        'Dim lcTabla				As String	= "'Cuentas_Pagar'"
        'Dim lcNombreOpcion		As String	= goServicios.mObtenerCampoFormatoSQL(Me.pcNombreOpcion)
        'Dim lcAccion			As String	= "'Eliminar'"
        'Dim lcDocumento			As String	= goServicios.mObtenerCampoFormatoSQL(lcNumero)
        'Dim lcCodigoRegistro	As String	= "'Sin código'"
        'Dim lcDetalle			As String	= goServicios.mObtenerCampoFormatoSQL(goAuditoria.KC_DetalleVacio)
        'Dim lcNombreEquipo		As String	= goServicios.mObtenerCampoFormatoSQL(goAuditoria.pcNombreEquipo)	
        'Dim lcCodigoObjeto		As String	= goServicios.mObtenerCampoFormatoSQL(TypeName(Me))
        'Dim lcNotas				As String	= "'Documento Eliminado desde complemento ""Eliminar Cuentas por Pagar"".'"
        'Dim lcClave2			As String	= goServicios.mObtenerCampoFormatoSQL(lcTipo)
        'Dim lcClave3			As String	= "''"
        'Dim lcClave4			As String	= "''"
        'Dim lcClave5			As String	= "''"

        'Dim lcInsercionAuditoria As String
        'lcInsercionAuditoria = goAuditoria.mObtenerCadenaGuardar(	lcTipoAuditoria,	_
        '														lcTabla,			_
        '														lcNombreOpcion,		_
        '														lcAccion,			_
        '														lcDocumento,		_
        '														lcCodigoRegistro,	_
        '														lcDetalle,			_
        '														lcNombreEquipo,		_
        '														lcCodigoObjeto,		_
        '														lcNotas,			_
        '														lcClave2, lcClave3, lcClave4, lcClave5) 


        '-------------------------------------------------------------------------------------------
        ' Generalmente se ejecuta mEjecutarOperacion() o mEjecutarTransaccion() en un	
        ' bloque TRY, mostrando el mensaje OK dentro del mismo, el mensaje de error en 
        ' el CATCH (con un RETURN que tetiene el procedimiento) y luego un segundo bloque 
        ' TRY que guarda las auditorias; en caso de error en el segundo TRY a veces 
        ' mostramos un mensaje y a veces falla en silencio (sin avisar al usuario); pero
        ' no se usa el RETURN.
        '-------------------------------------------------------------------------------------------

        '     Try	

        'loDatos.mEjecutarTransaccion(loTransacccion)

        'Muestra un mensaje tipo "Información" 
        'Me.mMostrarMensajeModal("Operación Completada", _
        '	"El Documento  '" & lcNumero & "' (" &  lcTipo &  ")" & _
        '	" fue eliminado satisfactoriamente. ", "i")

        '     Catch loExcepcion As Exception

        ''Un error al ejecutar la transaccion principal lo mostramos como mensaje tipo "Error"
        'Me.mMostrarMensajeModal("Operación no Completada", _
        '	"No fue posible completar la eliminación del documento. <br/>Información Adicional:" & _
        '	loExcepcion.Message, "e")

        'Return

        'End Try

        'Try

        'loDatos.mEjecutarComando(lcInsercionAuditoria)

        'Catch loExcepcion As Exception

        'Un error al guardar la auditoria lo mostramos como mensaje tipo "Advertencia" (si se muestra)
        'Me.mMostrarMensajeModal("Operación Completada", _
        '	"El Documento  '" & lcNumero & "' (" &  lcTipo &  ") " & _
        '	"fue eliminado satisfactoriamente, sin embargo no fue posible guardar el registro de auditoria. <br/>Información Adicional: " &  _
        '	loExcepcion.Message, "a")

        'End Try


        Me.txtDocumento.mLimpiarCampos()
        'Me.txtCod_Tip.mLimpiarCampos()
        Me.txtProveedor.Text = ""
        Me.cmdAceptar.Enabled = False
        Me.cmdCancelar.Text = "Cerrar"
	
	End Sub

    Protected Sub txtDocumento_mResultadoBusquedaNoValido(ByVal sender As vis1Controles.txtNormal, ByVal lcNombreCampo As String, ByVal lnIndice As Integer) Handles txtDocumento.mResultadoBusquedaNoValido

        Me.mMostrarMensajeModal("Advertencia", _
            "El Número de Documento indicado no es válido.", "a")

        Me.txtDocumento.pcTexto("Documento") = ""

        Me.txtProveedor.Text = ""

    End Sub

    Protected Sub txtDocumento_mResultadoBusquedaValido(ByVal sender As vis1Controles.txtNormal, ByVal lcNombreCampo As String, ByVal lnIndice As Integer) Handles txtDocumento.mResultadoBusquedaValido

        Dim lcNumero As String = Strings.Trim(Me.txtDocumento.pcTexto("Documento"))
        Dim lcUsuario As String = goUsuario.pcCodigo

        '-------------------------------------------------------------------------------------------
        ' Verifica que el usuario tenga permitido confirmar.
        '-------------------------------------------------------------------------------------------
        If (lcUsuario <> "mgentili" And lcUsuario <> "ssminanca" And lcUsuario <> "dmatheus" And lcUsuario <> "yreina" And lcUsuario <> "kodeitsu") Then

            Me.mMostrarMensajeModal("Operación no Completada", "Usted no tiene permisos para confirmar la orden de compra. ", "a")
            Return
        End If

        Dim loTabla As DataTable
        Dim loDatos As New goDatos
        Dim loConsulta As New StringBuilder()

        loConsulta.AppendLine("SELECT Ordenes_Compras.Status,")
        loConsulta.AppendLine("       Ordenes_Compras.Cod_Pro,")
        loConsulta.AppendLine("       Proveedores.Nom_Pro,")
        loConsulta.AppendLine("       Ordenes_Compras.logico1 AS mgentili,")
        loConsulta.AppendLine("       Ordenes_Compras.logico2 AS ssimanca,")
        loConsulta.AppendLine("       Ordenes_Compras.logico3 AS dmatheus,")
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
        ElseIf (CBool(loTabla.Rows(0).Item("dmatheus")) = True And lcUsuario = "dmatheus") Then
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
