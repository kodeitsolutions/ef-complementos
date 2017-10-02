<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CGS_frmConfirmarOrdenCompra.aspx.vb" Inherits="CGS_frmConfirmarOrdenCompra" %>
<%@ Register Assembly="vis1Controles" Namespace="vis1Controles" TagPrefix="vis1Controles" %>
<%@ Register Assembly="vis2Controles" Namespace="vis2Controles" TagPrefix="vis2Controles" %>
<%@ Register Assembly="vis3Controles" Namespace="vis3Controles" TagPrefix="vis3Controles" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Confirmar Orden de Compra</title>
    <link href="../../Framework/cssEstilosFramework.css" rel="stylesheet" type="text/css" />
    <link href="../../Administrativo/cssEstilosAdministrativo.css" rel="stylesheet" type="text/css" />  
    <link href="../../Framework/css/eFactory.css" rel="stylesheet" type="text/css" />
    
	<script type="text/javascript">
		jQuery(document).ready(function(){
		
			jQuery('#cmdCancelar').live('click', function(e){	
				e.preventDefault();
				window.close(); 
				return false;
			});
					
		//Cierra la ventana al presionar ESC
			jQuery(window).bind('keydown', function(e){
				if(e.which==27){
					e.preventDefault();
					window.close();
					return false;
				}
			});
			
		});
		
		window.resizeTo(550, 370);
		
	</script>
	
</head>
<body style="padding: 0px; margin: 4px 0px 0px 4px; border-collapse: collapse;">
    <form id="frmContenedor" runat="server">   
    <div>
            <asp:ScriptManager ID="spmActualizaciones" runat="server" AsyncPostBackTimeout="180">
				<Services>
					<asp:ServiceReference Path="~/Framework/Librerias/wbsServiciosDatos.asmx" />
				</Services>
			</asp:ScriptManager>
            <asp:UpdatePanel ID="udpPrincipal" runat="server" UpdateMode="Conditional">
            
                <ContentTemplate>
                    
                    <div class="divCuerpoComplemento">
                    <table style="border-collapse: collapse;table-layout: fixed; width: 507px;" >
                        <tr>
                            <td style="height: 4px; width: 150px;"></td>
							<td style="height: 4px; width: 400px"></td>
                        </tr>
                        <tr>
							<td class="SeparadorSeccionesFormularios" colspan="2">
                                <vis1Controles:lblNormal runat="server" ID="lblTitulo" Text="Eliminar Cuentas por Pagar" CssClass="TituloPanel" plBloqueadoParaEditar="False" plBloqueadoParaVer="False" plModoAmbiente="False" plPersonalizarHabilitado="False" plPersonalizarSeleccionado="False" plPersonalizarTitulo="True" plPersonalizarValor="False" plPersonalizarVisible="True" plSugeridoSeleccionado="False" pnTipoControl="KN_Etiqueta" pcNombreFormulario="" /></td>
                        </tr>
                        <tr>
                            <td style="height: 8px;" colspan="2"></td>
                        </tr>
						<tr>
							<td colspan="2"></td>
						</tr>
						<tr>
							<td>
								<vis1Controles:lblNormal ID="lblNumeroDocumento" runat="server" CssClass="Etiqueta" plBloqueadoParaEditar="False" plBloqueadoParaVer="False" plModoAmbiente="False" plPersonalizarHabilitado="False" plPersonalizarSeleccionado="False" plPersonalizarTitulo="True" plPersonalizarValor="False" plPersonalizarVisible="True" plSugeridoSeleccionado="False" pnTipoControl="KN_Etiqueta">Número de Documento:</vis1Controles:lblNormal></td>
							<td>
								<vis3Controles:txtCampoBusqueda ID="txtDocumento" runat="server" CssClass="CajasTexto" plAutoPostBack="True" plValidarAutomaticamente="True" pnNumeroCampos="1" poAncho="100px" />
							</td>
						</tr>
                        <tr>
							<td colspan="2" style="height: 4px"></td>
                        </tr>
                        <tr>
                            <td><vis1Controles:lblNormal runat="server" ID="lblProveedor" CssClass="Etiqueta" plBloqueadoParaEditar="False" plBloqueadoParaVer="False" plModoAmbiente="False" plPersonalizarHabilitado="False" plPersonalizarSeleccionado="False" plPersonalizarTitulo="True" plPersonalizarValor="False" plPersonalizarVisible="True" plSugeridoSeleccionado="False" pnTipoControl="KN_Etiqueta" pcNombreFormulario="">Proveedor:</vis1Controles:lblNormal></td>
							<td>
								<vis1Controles:txtNormal ID="txtProveedor" Width="350" runat="server" CssClass="CajasTexto" Enabled="False" plPermitirComillas="False" plSeleccionarAlObtenerFoco="True" ReadOnly="True"></vis1Controles:txtNormal></td>
                        </tr>
                    </table>

                    </div>
                    <div class="divBotoneraComplemento">
                        <vis1Controles:cmdNormal ID="cmdAceptar" runat="server" CssClass="BotonAceptar" Text="Aceptar" />
                        <vis1Controles:cmdNormal ID="cmdCancelar" runat="server" CssClass="BotonCancelar"
                            Text="Cancelar" OnClientClick="window.close(); return false;" />
                    </div>

                </ContentTemplate>
                
            </asp:UpdatePanel>
            
            <asp:UpdateProgress ID="uprProcesando" runat="server" AssociatedUpdatePanelID="udpPrincipal"
                DisplayAfter="250">
                <ProgressTemplate>
                    <div class="FondoVentanaModal" style="vertical-align: middle; position: fixed; text-align: center">
                    </div>
                    <div class='divProcesando'>
                        <vis1Controles:lblNormal ID="lblEsperar" runat="server" CssClass="EtiquetaProcesando" pcNombreFormulario=""
                            Style="padding-right: 1em; padding-left: 1em; background-color: white">Procesando...</vis1Controles:lblNormal></div>
                </ProgressTemplate>
            </asp:UpdateProgress>

				<vis3Controles:wbcAdministradorMensajeModal ID="wbcAdministradorMensajeModal" runat="server" />
				<vis3Controles:wbcAdministradorVentanaModal ID="WbcAdministradorVentanaModal" runat="server" />
				<vis3Controles:pnlVentanaModal ID="PnlVentanaModalOperacion" runat="server" pcEstiloBotonCerrar="BotonCerrarVentanaModal"
					pcEstiloFondo="FondoVentanaModal" pcEstiloMarco="MarcoVentanaModal" pcTextoBotonCerrar="Cerrar"
					plMostrarBotonCerrar="false" poAlto="520px" poAncho="550px" Style="left: -16px;
					top: 50px" />
				<vis3Controles:pnlMensajeModal ID="PnlMensajeModalOperacion" runat="server" pcEstiloContenido="ContenidoMensajeModal"
					pcEstiloFondo="FondoVentanaModal" pcEstiloTitulo="TituloMensajeModal" pcEstiloVentana="MarcoMensajeModal"
					poAlto="400px" poAncho="750px" poArriba="20%" poIzquierda="30%" />

    </div>
    </form>
</body>
</html>
