<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CGS_frmMaquila.aspx.vb" Inherits="CGS_frmMaquila" %>
<%@ Register Assembly="vis1Controles" Namespace="vis1Controles" TagPrefix="vis1Controles" %>
<%@ Register Assembly="vis2Controles" Namespace="vis2Controles" TagPrefix="vis2Controles" %>
<%@ Register Assembly="vis3Controles" Namespace="vis3Controles" TagPrefix="vis3Controles" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html">
<html>
<head runat="server">
    <title>Asistente de Maquila</title>
    <link href="../../Framework/cssEstilosFramework.css" rel="stylesheet" type="text/css" />
    <link href="../../Administrativo/cssEstilosAdministrativo.css" rel="stylesheet" type="text/css" />
    <link href="../../FrameWork/css/efactory.css" rel="stylesheet" />

    <style>
        .divContenedorGrid {
            margin: 0.5em;
        }
        .divContenedorGrid table{
            table-layout:auto;
        }
    </style>  
</head>
<body>
    <form id="frmContenedor" runat="server">
        <asp:ScriptManager ID="spmActualizaciones" runat="server" AsyncPostBackTimeout="180">
            <Services>
                <asp:ServiceReference Path="~/Framework/Librerias/wbsServiciosDatos.asmx" />
            </Services>
        </asp:ScriptManager>
        <asp:UpdatePanel ID="udpPrincipal" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="divCuerpoComplemento">

                    <table>
                        <thead>
                            <tr>
                                <td style="width: 100px; height: 4px;"></td>
                                <td style="width: 251px;"></td>
                                <td style="width: 251px;"></td>
                                <td style="width: 188px;"></td>
                            </tr>
                            <tr>
                                <td colspan="4" style="width: 100%;" class="SeparadorSeccionesFormularios">
                                    <vis1Controles:lblNormal runat="server" ID="lblTitulo"
                                        CssClass="TituloPanel" Text="Asistente de Maquila"></vis1Controles:lblNormal>
                                </td>
                            </tr>
                        </thead>
                            
                        <tbody>                           
                            <tr>
                                <td colspan="4" style="height: 6px;"></td>                                
                            </tr>
                            <tr></tr>  
                            <%--<tr>
                                <td style="vertical-align:top;"><vis1Controles:lblNormal runat="server" ID="lblComentario" Text="Comentario:" CssClass="Etiqueta"></vis1Controles:lblNormal></td>
                                <td>
                                    <vis1Controles:txtNormal runat="server" ID="TxtComentario" CssClass="CajasTexto" Width="400" Height="50px" Enabled="False" TextMode="MultiLine"></vis1Controles:txtNormal>
                                </td>
                            </tr>       --%>                 
                        </tbody>
                    </table>
                    <br />
                    <tr>
                        <td style="vertical-align:top;">
                            <vis1Controles:lblNormal runat="server" ID="lblComentCons" Text="Comentario Consumo:" CssClass="Etiqueta"></vis1Controles:lblNormal>
                        </td>
                        <td>
                            <vis1Controles:txtNormal runat="server" ID="TxtComentCons" CssClass="CajasTexto" Width="300" Enabled="True"></vis1Controles:txtNormal>
                        </td>
                    </tr>        
                    <br /><br />
                    <tr>
                        <td colspan="4" style="height: 10px;"></td>                                
                    </tr>
                    <tr></tr>

                    <tr>
                        <td>
                            <vis1Controles:lblNormal runat="server" ID="lblLCod_Alm" CssClass="Etiqueta" Text="Almacén de Consumo:"></vis1Controles:lblNormal>
                        </td>
                        <td>
                            <vis1Controles:cboNormal runat="server" ID="cboAlmacenConsumo" Width="100px" AutoPostBack="true" plPermitirVacio="false"></vis1Controles:cboNormal>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" style="height: 4px;"></td>
                    </tr>

                    <div class="divContenedorGrid">
                        
						<vis3Controles:grdListaRenglones ID="grdConsumido" runat="server" pcTitulo="Consumido" plPermitirActualizarRenglones="False"
							plPermitirAgregarRenglon="True" plPermitirEliminarRenglon="True" pnFilasVisibles="0"
							pnIndiceFilaSeleccionada="0" Width="82%" pnNumeroBotonesAdicionales="0" plBotonNuevaVentanaActivo="False" plBotonNuevaVentanaSiempreHabilitado="False" />
                        
                    </div>
                    <%--<tr>
                        <td colspan="2" style="height: 4px"></td>
                    </tr>
					<tr>
						<td colspan="2" style="vertical-align: top; height: 100px">
							<vis1Controles:lblNormal ID="lblAdvConsumido" runat="server" CssClass="Etiqueta" Font-Size="12pt" plBloqueadoParaEditar="False" plBloqueadoParaVer="False" plModoAmbiente="False" plPersonalizarHabilitado="False" plPersonalizarSeleccionado="False" plPersonalizarTitulo="True" plPersonalizarValor="False" plPersonalizarVisible="True" plSugeridoSeleccionado="False" pnTipoControl="KN_Etiqueta" pcNombreFormulario=""></vis1Controles:lblNormal></td>
					</tr>
                    <tr>
                        <td colspan="2" style="height: 4px;"></td>
                    </tr>--%>
                    <%--<br />--%>
                    <%--<tr>
                        <td colspan="2" style="height: 2px;"></td>
                    </tr>--%>
                    <%--<tr>
                        <td>
                            <vis1Controles:lblNormal runat="server" ID="lblTotal"
                                CssClass="Etiqueta" Text="Total:" Width="280px"></vis1Controles:lblNormal>
                        </td>                           
                        <td>
                            <vis2Controles:txtNumero runat="server" ID="txtTotal"
                                CssClass="CajasTextoNumero" Width="100px" plAceptarDecimales="True" plAceptarNegativos="False"
                                plLimitarValores="False" plPermitirComillas="False" plPermitirParentesisAngular="False" 
                                plSeleccionarAlObtenerFoco="True" plUsarSeparadorMiles="True" AutoPostBack="true" Enabled="false"></vis2Controles:txtNumero> 
                        </td>
                    </tr>--%>
                    <tr>
                        <td style="vertical-align:top;"><vis1Controles:lblNormal runat="server" ID="lblComentComentObt" Text="Comentario Obtenido:" CssClass="Etiqueta"></vis1Controles:lblNormal></td>
                        <td>
                            <vis1Controles:txtNormal runat="server" ID="TxtComentObt" CssClass="CajasTexto" Width="300" Enabled="True"></vis1Controles:txtNormal>
                        </td>
                    </tr> 
                    <br /><br />       
                    <tr>
                        <td colspan="2" style="height: 6px"></td>
                    </tr>
                    <tr>
                        <td>
                            <vis1Controles:lblNormal ID="lblAlmacen" runat="server" CssClass="Etiqueta">Almacén de Obtenido:</vis1Controles:lblNormal></td>
                        <td>
                            <vis1Controles:cboNormal runat="server" ID="cboAlmacenTrabajo" Width="80px" AutoPostBack="true" plPermitirVacio="false"></vis1Controles:cboNormal>
                        </td>
                    </tr>
                    <td colspan="2" style="height: 4px"></td>
                    <div class="divContenedorGrid">
                        
						<vis3Controles:grdListaRenglones ID="grdObtenido" runat="server" pcTitulo="Obtenido" plPermitirActualizarRenglones="False"
							plPermitirAgregarRenglon="True" plPermitirEliminarRenglon="True" pnFilasVisibles="0"
							pnIndiceFilaSeleccionada="0" Width="82%" pnNumeroBotonesAdicionales="0" plBotonNuevaVentanaActivo="False" plBotonNuevaVentanaSiempreHabilitado="False" />
                        
                    </div>
                    <tr>
                        <td colspan="2" style="height: 2px"></td>
                    </tr>
					<tr>
						<td colspan="2" style="vertical-align: top; height: 100px">
							<vis1Controles:lblNormal ID="lblNotificacion" runat="server" CssClass="Etiqueta" Font-Size="12pt" plBloqueadoParaEditar="False" plBloqueadoParaVer="False" plModoAmbiente="False" plPersonalizarHabilitado="False" plPersonalizarSeleccionado="False" plPersonalizarTitulo="True" plPersonalizarValor="False" plPersonalizarVisible="True" plSugeridoSeleccionado="False" pnTipoControl="KN_Etiqueta" pcNombreFormulario=""></vis1Controles:lblNormal></td>
					</tr>
                    <tr>
                        <td colspan="6" style="height: 4px;"></td>
                    </tr>


                </div>
                <div class="divBotoneraComplemento">
                    <vis1Controles:cmdNormal runat="server" ID="cmdAceptar" CssClass="BotonAceptar" Text="Aceptar" />
                    <vis1Controles:cmdNormal runat="server" ID="cmdCancelar" CssClass="BotonCancelar" Text="Cancelar" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

        <div style="position: absolute; bottom: 0;">

            <asp:UpdateProgress ID="uprProcesando" runat="server" AssociatedUpdatePanelID="udpPrincipal" DisplayAfter="250">
                <ProgressTemplate>
                    <div class="FondoVentanaModal"></div>
                    <div class='divProcesando'></div>
                </ProgressTemplate>
            </asp:UpdateProgress>

            <vis3Controles:wbcAdministradorMensajeModal ID="wbcAdministradorMensajeModal" runat="server" />
            <vis3Controles:wbcAdministradorVentanaModal ID="WbcAdministradorVentanaModal" runat="server" />
            <vis3Controles:pnlVentanaModal ID="PnlVentanaModalOperacion" runat="server"
                pcEstiloBotonCerrar="BotonCerrarVentanaModal" pcEstiloFondo="FondoVentanaModal"
                pcEstiloMarco="MarcoVentanaModal" pcTextoBotonCerrar="Cerrar" plMostrarBotonCerrar="false" />
            <vis3Controles:pnlMensajeModal ID="PnlMensajeModalOperacion" runat="server"
                pcEstiloContenido="ContenidoMensajeModal" pcEstiloFondo="FondoVentanaModal"
                pcEstiloTitulo="TituloMensajeModal" pcEstiloVentana="MarcoMensajeModal" />
        </div>


        <script src="../../Framework/Librerias/jquery.min.js"></script>
        <script src="../../FrameWork/Librerias/efactory.js"></script>
        <script type="text/javascript">

            jQuery(document).ready(function () {
                //Botón Cerrar 
                jQuery('body').on('click', '#cmdCancelar', function (e) {
                    e.preventDefault();
                    window.close();
                    return false;
                });


                jQuery('#udpPrincipal').on('click', '#cmdLimpiar', function (e) {

                    jQuery('.divContenedorGrid .CajasTexto').val('');
                    jQuery('.divContenedorGrid input[checked]').prop('checked', false).removeAttr('checked');
                    jQuery('.divContenedorGrid select').val('');
                    return true;

                });
            });
            //Tamaño inicial de la ventana
            window.resizeTo(650, 600);
        </script>

    </form>

</body>
</html>
