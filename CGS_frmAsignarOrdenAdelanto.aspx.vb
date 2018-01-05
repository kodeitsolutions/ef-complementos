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
Partial Class CGS_frmAsignarOrdenAdelanto
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
    Private Property Proveedor() As String
        Get
            Return CStr(Me.ViewState("Proveedor"))
        End Get
        Set(ByVal value As String)
            Me.ViewState("Proveedor") = value
        End Set
    End Property
    Private Property Monto() As Decimal
        Get
            Return CDec(Me.ViewState("Monto"))
        End Get
        Set(ByVal value As Decimal)
            Me.ViewState("Monto") = value
        End Set
    End Property
#End Region

#Region "Eventos"

    Protected Sub mCargaPagina(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'La primera vez que se cargue el formulario...
        If Not Me.IsPostBack() Then

            Me.TxtBusqueda.mConfigurarBusqueda("Ordenes_Compras", _
                                              "Documento", _
                                              "Documento,Comentario,status", _
                                              ".,Documento,Comentario,Estatus", _
                                              "Documento,Comentario,status", _
                                              "../../Framework/Formularios/frmFormularioBusqueda.aspx", _
                                              "Documento", _
                                              "", "Status = 'Confirmado'")

            Dim laParametros As Generic.Dictionary(Of String, Object)

            laParametros = Me.Session("frmComplementos.paParametros")
            Me.Session.Remove("frmComplementos.paParametros")

            If laParametros IsNot Nothing Then

                'Lee la colección de campos índice del formulario de origen
                Dim laIndices As Generic.Dictionary(Of String, Object)
                laIndices = laParametros("laIndices")

                Me.pcOrigenDocumento = CStr(laIndices("Documento")).Trim()

            End If
        End If

        If Not Me.IsPostBack() Then

            Dim loConsulta As New StringBuilder()

            loConsulta.AppendLine("SELECT	Renglones_Pagos.Cod_Tip	AS Cod_Tip,")
            loConsulta.AppendLine("		    Renglones_Pagos.Doc_Ori	AS Doc_Ori,")
            loConsulta.AppendLine("		    Renglones_Pagos.Mon_Abo	AS Mon_Abo,")
            loConsulta.AppendLine("         Pagos.Ord_Com           AS Ord_Com,")
            loConsulta.AppendLine("         Pagos.Cod_Pro           AS Cod_Pro")
            loConsulta.AppendLine("FROM Renglones_Pagos")
            loConsulta.AppendLine(" JOIN Pagos ON Pagos.Documento = Renglones_Pagos.Documento")
            loConsulta.AppendLine("WHERE Renglones_Pagos.Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento))
            loConsulta.AppendLine("")

            Dim loRenglones As DataTable = (New goDatos()).mObtenerTodosSinEsquema(loConsulta.ToString(), "Renglones_Recepciones").Tables(0)

            'SI TRAE MAS DE UN RENGLON SE SABE QUE NO CORRESPONDE A UN ADELANTO
            If loRenglones.Rows.Count > 1 Then
                Me.mMostrarMensajeModal("Operación no permitida", "Solo se puede asignar la orden de compra a un adelanto.", "a")
                Me.cmdAceptar.Enabled = False
                Return
            Else
                'SE VERIFICA EL TIPO DE DOCUMENTO EN EL PAGO PARA CORROBORAR QUE SEA UN ADELANTO
                If CStr(loRenglones.Rows(0).Item("Cod_Tip")).Trim() <> "ADEL" Then
                    Me.mMostrarMensajeModal("Operación no permitida", "Solo se puede asignar la orden de compra a un adelanto.", "a")
                    Me.cmdAceptar.Enabled = False
                    Return
                End If
            End If

            'SI EL ADELANTO YA TIENE UNA ORDEN DE COMPRA SE NOTIFICA CON UNA ADVERTENCIA
            If CStr(loRenglones.Rows(0).Item("Ord_Com")).Trim() <> "" Then
                Me.lblAdvertencia.Text = "Este adelanto ya tiene asociada la orden de compra " & CStr(loRenglones.Rows(0).Item("Ord_Com")).Trim() & "."
            End If

            Me.Monto = CDec(loRenglones.Rows(0).Item("Mon_Abo"))
            Me.Proveedor = CStr(loRenglones.Rows(0).Item("Cod_Pro")).Trim()

        End If

    End Sub

    Protected Sub cmdAceptar_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles cmdAceptar.Click


        Dim loConsulta As New StringBuilder()

        loConsulta.AppendLine("UPDATE Pagos SET Ord_Com = " & goServicios.mObtenerCampoFormatoSQL(Me.TxtBusqueda.pcTexto("Documento")))
        loConsulta.AppendLine("WHERE Documento = " & goServicios.mObtenerCampoFormatoSQL(Me.pcOrigenDocumento))
        loConsulta.AppendLine("")
        loConsulta.AppendLine("")

        Dim lodatos As New goDatos()

        Try
            Dim laSentencias As New ArrayList()
            laSentencias.Add(loConsulta.ToString())
            'EJECUTAR EL UPDATE PARA GUARDAR EL NÚMERO DE ORDEN DE COMPRA. EL VALOR SE GUARDA EN EL CAMPO ord_com DE LA TABLA PAGOS
            lodatos.mEjecutarTransaccion(laSentencias)

            Me.mMostrarMensajeModal("Origen asignado", "Se asignó la orden de compra " & Me.TxtBusqueda.pcTexto("Documento") & " como origen. ", "i", False)

        Catch ex As Exception

            Me.mMostrarMensajeModal("Proceso no completado", "No fue posible asignar la orden de compra.", "e", True)

        End Try
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

        'COLOCAR COMENTARIO, PROVEEDOR Y CALCULAR 70 % DEL MONTO BRUTO DE LA ORDEN DE COMPRA
        Me.cmdAceptar.Enabled = True

        Dim lcDocumento As String = goServicios.mObtenerCampoFormatoSQL(Me.TxtBusqueda.pcTexto("Documento"))

        Dim lcConsultaOrden As String = "SELECT Comentario, Mon_Bru * 0.7 AS Monto_Orden, Cod_Pro FROM Ordenes_Compras WHERE Documento = " & lcDocumento

        Dim loTConsulta As DataTable = (New goDatos()).mObtenerTodosSinEsquema(lcConsultaOrden, "Orden").Tables(0)

        Me.TxtComentario.Text = CStr(loTConsulta.Rows(0).Item("Comentario")).Trim()

        'SE VERIFICA QUE EL PROVEEDOR DE LA ORDEN DE COMPRA COINCIDA CON EL DEL PAGO
        If Me.Proveedor <> CStr(loTConsulta.Rows(0).Item("Cod_Pro")).Trim() Then
            Me.mMostrarMensajeModal("Operación no permitida", "El proveedor de la orden de compra es distinto al del adelanto.", "a")
            Me.cmdAceptar.Enabled = False
        End If

        'TRAER VALOR DE LA OPCION "VALADLODC" QUE VERIFICA SI SE DEBE VALIDAR QUE EL ADELANTO SEA MAYOR AL MONTO BRUTO DE LA ORDEN
        'SE PERMITE HASTA EL 70 % DEL MONTO BRUTO
        Dim llValidar As Boolean = CBool(goOpciones.mObtener("VALADLODC", ""))

        If llValidar Then
            If Me.Monto > CDec(loTConsulta.Rows(0).Item("Monto_Orden")) Then
                Me.mMostrarMensajeModal("Operación no permitida", "El monto del adelanto es mayor al 70 % de la orden de compra.", "a")
                Me.cmdAceptar.Enabled = False
                Return
            End If
        End If
    End Sub

#End Region

End Class
'-------------------------------------------------------------------------------------------'
' Fin del codigo																			'
'-------------------------------------------------------------------------------------------'
' KDE: 27/10/17: Codigo Inicial.								                            '
'-------------------------------------------------------------------------------------------'
' 
