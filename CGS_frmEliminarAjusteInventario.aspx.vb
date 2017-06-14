﻿'-------------------------------------------------------------------------------------------'
' Inicio del codigo
'-------------------------------------------------------------------------------------------'
Imports vis3Controles.wbcAdministradorMensajeModal
Imports Microsoft.VisualBasic
Imports cusAplicacion
Imports cusDatos
Imports System.Data

'-------------------------------------------------------------------------------------------'
' Inicio de clase "CGS_frmEliminarAjusteInventario"
'-------------------------------------------------------------------------------------------'
Partial Class CGS_frmEliminarAjusteInventario
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


#End Region

#Region "Eventos"

    Protected Sub mCargaPagina(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'La primera vez que se cargue el formulario...
        If Not Me.IsPostBack() Then

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
                Me.txtDocumento.Text = Me.pcOrigenDocumento

                Me.mCargarDocumento(Me.pcOrigenDocumento)

            End If

        End If

    End Sub

    Protected Sub cmdAceptar_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles cmdAceptar.Click


        Dim lcDocumentoSQL As String = goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento)
        Dim loConsulta As New StringBuilder()
        Dim loDatos As New goDatos()

        Dim loTransacccion As New ArrayList()

        loConsulta.AppendLine("DECLARE @lcDocumento AS VARCHAR(10) = " & lcDocumentoSQL)
        loConsulta.AppendLine("DECLARE @lnCantidad AS DECIMAL(28,3) = (SELECT can_art1 FROM Renglones_Ajustes WHERE Documento = @lcDocumento)")
        loConsulta.AppendLine("DECLARE @lcTipo AS VARCHAR(15) = (SELECT Tipo FROM Renglones_Ajustes WHERE Documento = @lcDocumento)")
        loConsulta.AppendLine("DECLARE @lcArticulo AS VARCHAR(8) = (SELECT Cod_Art FROM Renglones_Ajustes WHERE Documento = @lcDocumento)")
        loConsulta.AppendLine("DECLARE @lcAlmacen AS VARCHAR(15) = (SELECT Cod_Alm FROM Renglones_Ajustes WHERE Documento = @lcDocumento)")
        loConsulta.AppendLine("DECLARE @lcLote AS VARCHAR(30) = COALESCE((SELECT Cod_Lot FROM Operaciones_Lotes WHERE Num_Doc = @lcDocumento AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("								  AND Tip_Doc = 'Ajustes_Inventarios' AND Tip_Ope = @lcTipo AND Cod_Alm = @lcAlmacen),'')")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("IF RTRIM(@lcTipo) = 'Entrada'")
        loConsulta.AppendLine("	SET @lnCantidad = @lnCantidad * (-1)")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("UPDATE Articulos SET Exi_Act1 = Exi_Act1 + @lnCantidad WHERE Cod_Art = @lcArticulo")
        loConsulta.AppendLine("UPDATE Renglones_Almacenes SET Exi_Act1 = Exi_Act1 + @lnCantidad WHERE Cod_Art = @lcArticulo AND Cod_Alm = @lcAlmacen")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("IF @lclote <> ''")
        loConsulta.AppendLine("BEGIN")
        loConsulta.AppendLine("	UPDATE Renglones_Lotes SET Exi_Act1 = Exi_Act1 + @lnCantidad ")
        loConsulta.AppendLine("	WHERE Cod_Art = @lcArticulo AND Cod_Alm = @lcAlmacen AND Cod_Lot = @lcLote")
        loConsulta.AppendLine("	DELETE FROM Operaciones_Lotes WHERE Num_Doc = @lcDocumento AND Cod_Art = @lcArticulo")
        loConsulta.AppendLine("								  AND Tip_Doc = 'Ajustes_Inventarios' AND Tip_Ope = @lcTipo AND Cod_Alm = @lcAlmacen")
        loConsulta.AppendLine("END")
        loConsulta.AppendLine("")
        loConsulta.AppendLine("DELETE FROM Operaciones")
        loConsulta.AppendLine("DELETE FROM Renglones_Ajustes WHERE Documento = @lcDocumento")
        loConsulta.AppendLine("DELETE FROM Ajustes WHERE Documento = @lcDocumento")


        loTransacccion.Add(loConsulta.ToString())
        loConsulta.Length = 0

        Dim lcTipoAuditoria As String = "'Datos'"
        Dim lcTabla As String = "'Ajustes'"
        Dim lcNombreOpcion As String = "'CGS_frmEliminarAjusteInventario.aspx'"
        Dim lcAccion As String = "'Eliminar'"
        Dim lcDocumento As String = goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento)
        Dim lcCodigoRegistro As String = "'Sin código'"
        Dim lcDetalle As String = goServicios.mObtenerCampoFormatoSQL(goAuditoria.KC_DetalleVacio)
        Dim lcNombreEquipo As String = goServicios.mObtenerCampoFormatoSQL(goAuditoria.pcNombreEquipo)
        Dim lcCodigoObjeto As String = goServicios.mObtenerCampoFormatoSQL(TypeName(Me))
        Dim lcNotas As String = "'Documento Eliminado desde complemento ""Eliminar Ajuste de Inventario (CGS)"".'"
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

            loDatos.mEjecutarTransaccion(loTransacccion)

            'Muestra un mensaje tipo "Información" 
            Me.mMostrarMensajeModal("Operación Completada", _
                "El Documento  '" & Me.pcOrigenDocumento & " fue eliminado satisfactoriamente. ", "i")

        Catch loExcepcion As Exception

            'Un error al ejecutar la transaccion principal lo mostramos como mensaje tipo "Error"
            Me.mMostrarMensajeModal("Operación no Completada", _
                "No fue posible completar la eliminación del documento. <br/>Información Adicional:" & _
                loExcepcion.Message, "e")

            Return

        End Try

        Try

            loDatos.mEjecutarComando(lcInsercionAuditoria)

        Catch loExcepcion As Exception

            'Un error al guardar la auditoria lo mostramos como mensaje tipo "Advertencia" (si se muestra)
            Me.mMostrarMensajeModal("Operación Completada", _
                "El Documento  '" & Me.pcOrigenDocumento & _
                " fue eliminado satisfactoriamente, sin embargo no fue posible guardar el registro de auditoria. <br/>Información Adicional: " & _
                loExcepcion.Message, "a")

        End Try

    End Sub

#End Region

#Region "Metodos"

    '    '    ''' <summary>
    '    '    ''' Deshabilita los controles del formulario si el cambio de tasa no es 
    '    '    ''' posible para el documento indicado. 
    '    '    ''' </summary>
    '    '    ''' <remarks></remarks>
    Private Sub mDeshabilitarTodo()

        Me.cmdAceptar.Visible = False

    End Sub

    '    '    ''' <summary>
    '    '    ''' Carga los datos del docmetno indicado y valida si es posible modificar la tasa del mismo. 
    '    '    ''' </summary>
    '    '    ''' <param name="lcDocumento"></param>
    '    '    ''' <param name="lcTabla"></param>
    '    '    ''' <remarks></remarks>
    Private Sub mCargarDocumento(ByVal lcDocumento As String)

        Dim lcDocumentoSQL As String = goServicios.mObtenerCampoFormatoSQL(lcDocumento)
        Me.txtDocumento.Text = lcDocumento

        Dim loDatosBusqueda As New goDatos()
        Dim loQuery As New StringBuilder()

        loQuery.AppendLine("SELECT Documento, Comentario, Status")
        loQuery.AppendLine("FROM Ajustes")
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

        If CStr(loFilaQuery("Status")).Trim() = "Pendiente" Then
            Me.mMostrarMensajeModal("Documento Bloqueado", "Documento en estatus 'Pendiente'. Elimine este ajuste usando el botón eliminar en la ficha General.", "a")

            Me.mDeshabilitarTodo()
            Return
        End If

    End Sub

    '    '    ''' <summary>
    '    '    ''' Muestra un mensaje modal en pantalla.
    '    '    ''' </summary>
    '    '    ''' <param name="lcTitulo"></param>
    '    '    ''' <param name="lcContenido"></param>
    '    '    ''' <param name="lcTipo"></param>
    '    '    ''' <remarks></remarks>
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
'''-------------------------------------------------------------------------------------------'
''' Fin del codigo																			'
'''-------------------------------------------------------------------------------------------'
''' RJG: 15/12/15: Codigo Inicial.								                            '
'''-------------------------------------------------------------------------------------------'
