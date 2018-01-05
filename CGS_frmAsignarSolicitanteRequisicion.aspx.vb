'-------------------------------------------------------------------------------------------'
' Inicio del codigo
'-------------------------------------------------------------------------------------------'
Imports vis3Controles.wbcAdministradorMensajeModal
Imports Microsoft.VisualBasic

'-------------------------------------------------------------------------------------------'
' Inicio de clase "CGS_frmAsignarSolicitanteRequisicion"
'-------------------------------------------------------------------------------------------'
Partial Class CGS_frmAsignarSolicitanteRequisicion
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


            Me.txtDocumento.mConfigurarBusqueda("Requisiciones", _
                                              "Documento", _
                                              "Documento,Comentario,status", _
                                              ".,Documento,Comentario,Estatus", _
                                              "Documento,Comentario,status", _
                                              "../../Framework/Formularios/frmFormularioBusqueda.aspx", _
                                              "Documento", _
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

        Dim loDatos As New goDatos()
        Dim loSentencias As New StringBuilder()
        Dim loTransacccion As New ArrayList()

        'LOS DATOS DE LOS SOLICITANTES SE GUARDAN EN LOS CAMPOS caracterX DE LA TABLA REQUISICIONES
        loSentencias.AppendLine("UPDATE Requisiciones")
        loSentencias.AppendLine("SET Caracter1 = " & goServicios.mObtenerCampoFormatoSQL(Me.txtSolicitante1.Text) & ",")
        loSentencias.AppendLine("Caracter2 = " & goServicios.mObtenerCampoFormatoSQL(Me.txtSolicitante2.Text) & ",")
        loSentencias.AppendLine("Caracter3 = " & goServicios.mObtenerCampoFormatoSQL(Me.txtSolicitante3.Text) & ",")
        loSentencias.AppendLine("Caracter4 = " & goServicios.mObtenerCampoFormatoSQL(Me.txtOtros.Text))
        loSentencias.AppendLine("WHERE Documento = " & lcNumero)
        loSentencias.AppendLine("")
        loSentencias.AppendLine("")
        loSentencias.AppendLine("")
        loSentencias.AppendLine("")

        loTransacccion.Add(loSentencias.ToString())

        Try
            loDatos.mEjecutarTransaccion(loTransacccion)
            'Muestra un mensaje tipo "Información" 
            Me.mMostrarMensajeModal("Operación Completada", "Datos de solicitante(s) guardados. ", "i")
        Catch loExcepcion As Exception
            'Un error al ejecutar la transaccion principal lo mostramos como mensaje tipo "Error"
            Me.mMostrarMensajeModal("Operación no Completada", _
                "No fue posible completar la confirmación del documento. <br/>Información Adicional:" & _
                loExcepcion.Message, "e")
            Me.cmdAceptar.Enabled = False
            Me.cmdCancelar.Text = "Cerrar"
            Return
        End Try

        Me.txtDocumento.mLimpiarCampos()
        Me.txtProveedor.Text = ""
        Me.TxtComentario.Text = ""
        Me.txtSolicitante1.Text = ""
        Me.txtSolicitante2.Text = ""
        Me.txtSolicitante3.Text = ""
        Me.txtOtros.Text = ""
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
        Dim loTabla As DataTable
        Dim loDatos As New goDatos
        Dim loConsulta As New StringBuilder()

        'SI YA TIENE SOLICITANTES ASOCIADOS LOS CARGA EN SUS RESPECTIVOS CAMPOS DE LA PANTALLA
        loConsulta.AppendLine("SELECT Requisiciones.Cod_Pro,")
        loConsulta.AppendLine("       Requisiciones.Comentario,")
        loConsulta.AppendLine("       Proveedores.Nom_Pro,")
        loConsulta.AppendLine("       Requisiciones.Caracter1 AS Solicitante1,")
        loConsulta.AppendLine("       Requisiciones.Caracter2 AS Solicitante2,")
        loConsulta.AppendLine("       Requisiciones.Caracter3 AS Solicitante3,")
        loConsulta.AppendLine("       Requisiciones.Caracter4 AS Otros")
        loConsulta.AppendLine("FROM Requisiciones")
        loConsulta.AppendLine(" JOIN Proveedores ON Proveedores.Cod_Pro = Requisiciones.Cod_Pro")
        loConsulta.AppendLine("WHERE Requisiciones.Documento = " & goServicios.mObtenerCampoFormatoSQL(lcNumero))
        loConsulta.AppendLine("")

        loTabla = loDatos.mObtenerTodosSinEsquema(loConsulta.ToString(), "Ordenes_Compras").Tables(0)

        Me.txtProveedor.Text = CStr(loTabla.Rows(0).Item("Cod_Pro")).Trim() & " - " & CStr(loTabla.Rows(0).Item("Nom_Pro")).Trim()
        Me.TxtComentario.Text = CStr(loTabla.Rows(0).Item("Comentario")).Trim()
        Me.txtSolicitante1.Text = CStr(loTabla.Rows(0).Item("Solicitante1")).Trim()
        Me.txtSolicitante2.Text = CStr(loTabla.Rows(0).Item("Solicitante2")).Trim()
        Me.txtSolicitante3.Text = CStr(loTabla.Rows(0).Item("Solicitante3")).Trim()
        Me.txtOtros.Text = CStr(loTabla.Rows(0).Item("Otros")).Trim()

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
' KDE: 09/11/17: Codigo Inicial																'
'-------------------------------------------------------------------------------------------'
