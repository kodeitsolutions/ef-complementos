<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CGS_frmEliminarAjusteInventario.aspx.vb" Inherits="CGS_frmEliminarAjusteInventario" %>
<%@ Register Assembly="vis1Controles" Namespace="vis1Controles" TagPrefix="vis1Controles" %>
<%@ Register Assembly="vis2Controles" Namespace="vis2Controles" TagPrefix="vis2Controles" %>
<%@ Register Assembly="vis3Controles" Namespace="vis3Controles" TagPrefix="vis3Controles" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html">
<html>
<head runat="server">
    <title>Eliminar Ajuste de Inventario (CGS)</title>
    <link href="../../Framework/cssEstilosFramework.css" rel="stylesheet" type="text/css" />
    <link href="../../Administrativo/cssEstilosAdministrativo.css" rel="stylesheet" type="text/css" />
    <link href="../../FrameWork/css/efactory.css" rel="stylesheet" />

    <style>
        .divCuerpoComplemento {
            margin:0.5em;
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
                                <th style="height: 4px; width: 90px;"></th>
                                <th style="width: 110px;"></th>
                                <th style="width: 60px;"></th>
                                <th style="width: 100px;"></th>
                                <th style="width: 60px;"></th>
                                <th></th>
                            </tr>
                            <tr>
                                <th colspan="6" class="SeparadorSeccionesFormularios">
                                    <vis1Controles:lblNormal runat="server" ID="lblTitulo"
                                        CssClass="TituloPanel" Text="Eliminar Ajuste de Inventario"></vis1Controles:lblNormal>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td colspan="6" style="height: 8px;"></td>
                            </tr>
                            <tr>
                                <td>
                                    <vis1Controles:lblNormal runat="server" ID="lblDocumento"
                                        CssClass="Etiqueta" Text="Documento:"></vis1Controles:lblNormal>
                                </td>                            
                                <td>
                                    <vis1Controles:txtNormal runat="server" ID="txtDocumento"
                                        CssClass="CajasTexto" Enabled ="false" Width="100px"></vis1Controles:txtNormal>
                                </td>                                
                            </tr>    
                            <tr>
                                <td colspan="6" style="height: 4px;"></td>
                            </tr>
                            <tr>
                                <td colspan="6" style="height: 4px;"></td>
                            </tr>
                            <tr>
                                <td style="vertical-align:top;">
                                    <vis1Controles:lblNormal runat="server" ID="lblComentario"
                                        CssClass="Etiqueta" Text="Comentario:"></vis1Controles:lblNormal>
                                </td>
                                <td>
                                    <vis1Controles:txtNormal runat="server" ID="txtComentario"
                                        CssClass="CajasTexto" Enabled ="false" Width="325px" Height="50px" TextMode="MultiLine"></vis1Controles:txtNormal>
                                </td>                           
                            </tr>
                            <tr>
                                <td colspan="2" style="height: 4px"></td>
                            </tr>
						    <tr>
							    <td colspan="2" style="vertical-align: top; height: 100px">
								    <vis1Controles:lblNormal ID="lblAdvertencia" runat="server" CssClass="Etiqueta" Font-Size="12pt" plBloqueadoParaEditar="False" plBloqueadoParaVer="False" plModoAmbiente="False" plPersonalizarHabilitado="False" plPersonalizarSeleccionado="False" plPersonalizarTitulo="True" plPersonalizarValor="False" plPersonalizarVisible="True" plSugeridoSeleccionado="False" pnTipoControl="KN_Etiqueta" pcNombreFormulario="">Verifique el número de documento a eliminar. Esta operación no se puede deshacer. </vis1Controles:lblNormal></td>
						    </tr>
                            <tr>
                                <td colspan="6" style="height: 4px;"></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="6" style="height: 4px;"></td>
                            </tr>
                        </tbody>
                    </table>
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
    </form>

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
            //Cierra la ventana al presionar ESC
            jQuery(document).bind('keydown', function (e) {
                if (e.which == 27) {
                    e.preventDefault();
                    window.close();
                    return false;
                }
            });
        });
        //Tamaño inicial de la ventana
        window.resizeTo(600, 400);
    </script>

</body>
</html>
