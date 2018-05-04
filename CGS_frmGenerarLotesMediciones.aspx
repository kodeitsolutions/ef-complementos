<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CGS_frmGenerarLotesMediciones.aspx.vb" Inherits="CGS_frmGenerarLotesMediciones" %>
<%@ Register Assembly="vis1Controles" Namespace="vis1Controles" TagPrefix="vis1Controles" %>
<%@ Register Assembly="vis2Controles" Namespace="vis2Controles" TagPrefix="vis2Controles" %>
<%@ Register Assembly="vis3Controles" Namespace="vis3Controles" TagPrefix="vis3Controles" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html">
<html>
<head runat="server">
    <title>Generar Lotes y Mediciones (CGS)</title>
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
        #grdArticulos_Renglones {
            height: calc(100vh - 8em) !important;
        }
        /* FIX para compatibilidad con hoja de estilos vieja */
        /*
        *, *::before, *::after {
            box-sizing: content-box;
        }
        body {
            padding: 4px 0 0 4px !important;
        }
        input {
            box-sizing: border-box !important;
        }*/
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
                        <tbody>
                            <tr>
                                <td style="width: 71px; height: 4px;"></td>
                                <td style="width: 779px;"></td>
                            </tr>
                            <tr>
                                <td colspan="4" style="width: 100%;" class="SeparadorSeccionesFormularios">
                                    <vis1Controles:lblNormal runat="server" ID="lblTitulo"
                                        CssClass="TituloPanel" Text=""></vis1Controles:lblNormal>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="height: 8px;"></td>
                            </tr>
                            <tr>
                                <!--<td>
                                    <vis1Controles:lblNormal runat="server" ID="lblDocumento"
                                        CssClass="Etiqueta" Text="Documento:"></vis1Controles:lblNormal>
                                </td>-->
                                <td>
                                    <vis1Controles:lblNormal runat="server" ID="lblLArticulo"
                                        CssClass="Etiqueta" Text="Artículo:"></vis1Controles:lblNormal>
                                </td>
                                <td>
                                    <vis1Controles:lblNormal runat="server" ID="lblArticulo"
                                        CssClass="Etiqueta" Text=""></vis1Controles:lblNormal>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="height: 4px;"></td>
                            </tr>
                            <tr> 
                                <td>
                                    <vis1Controles:lblNormal runat="server" ID="lblDAlmacen"
                                        CssClass="Etiqueta" Text="Almacén:"></vis1Controles:lblNormal>
                                </td>
                                <td>
                                    <vis1Controles:lblNormal runat="server" ID="lblAlmacen"
                                        CssClass="Etiqueta" Text=""></vis1Controles:lblNormal>
                                </td>                               
                                
                            </tr>
                            <tr>
                                <td colspan="4" style="height: 4px;"></td>
                            </tr>
                            <tr>                                     
                                <td>
                                    <vis1Controles:lblNormal runat="server" ID="lblDRenglon"
                                        CssClass="Etiqueta" Text="Renglón:"></vis1Controles:lblNormal>
                                </td>
                                <td>
                                    <vis1Controles:lblNormal runat="server" ID="lblRenglon"
                                        CssClass="Etiqueta" Text=""></vis1Controles:lblNormal>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="height: 4px;"></td>
                            </tr>
                            <tr>  
                                <td>
                                    <vis1Controles:lblNormal runat="server" ID="lblDCantidad"
                                        CssClass="Etiqueta" Text="Cantidad: "></vis1Controles:lblNormal>
                                </td>
                                <td>
                                    <vis1Controles:lblNormal runat="server" ID="lblCantidad"
                                        CssClass="Etiqueta" Text=""></vis1Controles:lblNormal>
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <div class="divContenedorGrid">
                        
						<vis3Controles:grdListaRenglones ID="grdLotesMediciones" runat="server" pcTitulo="Lotes y Mediciones" plPermitirActualizarRenglones="True"
							plPermitirAgregarRenglon="True" plPermitirEliminarRenglon="True" pnFilasVisibles="0"
							pnIndiceFilaSeleccionada="0" Width="100%" pnNumeroBotonesAdicionales="1" plBotonNuevaVentanaActivo="False" plBotonNuevaVentanaSiempreHabilitado="False" />
                        <%--<tr>
                                <td colspan="6" style="height: 4px;"></td>
                            </tr>
                            <tr>
                                <td>
                                    <vis1Controles:lblNormal runat="server" ID="lblTotal"
                                        CssClass="Etiqueta" Text="Total:" Width="150px"></vis1Controles:lblNormal>
                                </td>                           
                                <td>
                                    <vis2Controles:txtNumero runat="server" ID="txtTotal"
                                        CssClass="CajasTextoNumero" Width="100px" plAceptarDecimales="True" plAceptarNegativos="False"
                                        plLimitarValores="False" plPermitirComillas="False" plPermitirParentesisAngular="False" 
                                        plSeleccionarAlObtenerFoco="True" plUsarSeparadorMiles="True" AutoPostBack="true" Enabled="false"></vis2Controles:txtNumero> 
                                </td>
                            </tr>--%>
                        <%--<tr>
                            <td colspan="2" style="height: 4px"></td>
                        </tr>
						<tr>
							<td colspan="2" style="vertical-align: top; height: 100px">
								<vis1Controles:lblNormal ID="lblAdvertencia" runat="server" CssClass="Etiqueta" Font-Size="11pt" plBloqueadoParaEditar="False" plBloqueadoParaVer="False" plModoAmbiente="False" plPersonalizarHabilitado="False" plPersonalizarSeleccionado="False" plPersonalizarTitulo="True" plPersonalizarValor="False" plPersonalizarVisible="True" plSugeridoSeleccionado="False" pnTipoControl="KN_Etiqueta" pcNombreFormulario="">Si el lote/colada ya existe no se generarán mediciones. </vis1Controles:lblNormal></td>
						</tr>--%>
                    </div>


                </div>
                <div class="divBotoneraComplemento">
                    <vis1Controles:cmdNormal runat="server" ID="cmdLimpiar" CssClass="BotonLimpiar" Text="Limpiar" style="float:left;" ToolTip="Borra la pantalla y carga los renglones iniciales vacios" />
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
        
            jQuery( document ).ready( function () {
                //Botón Cerrar 
                jQuery( 'body' ).on( 'click', '#cmdCancelar', function ( e ) {
                    e.preventDefault();
                    window.close();
                    return false;
                } );         


                jQuery( '#udpPrincipal' ).on( 'click', '#cmdLimpiar', function ( e ) {

                    jQuery( '.divContenedorGrid .CajasTexto' ).val( '' );
                    jQuery( '.divContenedorGrid input[checked]' ).prop( 'checked', false ).removeAttr( 'checked' );
                    jQuery( '.divContenedorGrid select' ).val( '' );
                    return true;

                } );
            } );
            //Tamaño inicial de la ventana
           window.resizeTo(850, 600);
        </script>

    </form>

</body>
</html>
