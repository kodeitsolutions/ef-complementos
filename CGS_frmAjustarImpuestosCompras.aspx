<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CGS_frmAjustarImpuestosCompras.aspx.vb" Inherits="CGS_frmAjustarImpuestosCompras" %>

<%@ Register Assembly="vis1Controles" Namespace="vis1Controles" TagPrefix="vis1Controles" %>
<%@ Register Assembly="vis2Controles" Namespace="vis2Controles" TagPrefix="vis2Controles" %>
<%@ Register Assembly="vis3Controles" Namespace="vis3Controles" TagPrefix="vis3Controles" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Ajustar Impuestos Facturas de Compras</title>
	<link href="../../Framework/cssEstilosFramework.css" rel="stylesheet" type="text/css" />
	<link href="../../Administrativo/cssEstilosAdministrativo.css" rel="stylesheet" type="text/css" />
    <link href="../../Framework/css/efactory.css" rel="stylesheet" type="text/css" />
    <style>
        .divCuerpoComplemento {
            margin-left: 0.5em;
        }
    </style>

	<script type="text/javascript">
		
		jQuery(document).ready(function(e){
			
		//***************************************************************************
		// Captura los errores no controlados (una actualización cancelada al cerrar*
		// el formulario).															*
		//***************************************************************************
			Sys.WebForms.PageRequestManager.getInstance().add_endRequest(
				function mManejadorRespuestas(sender, args){
				   if (args.get_error() != undefined)
				   {
					   args.set_errorHandled(true);
				   }
				}
			);

			jQuery('#cmdCancelar').live('click', function(e){
				window.close();
				return false;
			});
			
			jQuery(window).keyup(function(e){
				if (e.keyCode == 27){
					jQuery('#cmdCancelar').click();
				}
			})
						
			jQuery('#cboImpuestoAnterior').focus();
			
		});
        
		//window.poOrigen = (!!window.opener)? ( (!!window.opener.opener) ? window.opener.opener: null) : null;
        window.resizeTo(600, 480)
	</script>

</head>
<body>
	<form id="frmContenedor" runat="server">

		<div style="margin-left:4px;">
			<asp:ScriptManager ID="spmOperaciones" runat="server" AsyncPostBackTimeout="9000">
				<Services>
					<asp:ServiceReference Path="~/FrameWork/Librerias/wbsServiciosDatos.asmx" />
				</Services>
			</asp:ScriptManager>
			<asp:UpdatePanel ID="udpActualizaciones" runat="server">
				<ContentTemplate>
                    <div class="divCuerpoComplemento">

                        <table style="width: calc(100% - 16px);">
                            <thead>
                                <tr>
                                    <td style="height: 8px; width: 70px;"></td>
                                    <td style="height: 8px; width: 120px;"></td>
                                    <td style="height: 8px; width: 80px;"></td>
                                    <td style="height: 8px; width: 70px;"></td>
                                    <td style="height: 8px;"></td>
                                    <td style="height: 8px; width: 80px;"></td>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td class="SeparadorSeccionesFormularios" colspan="6">
                                        <vis1Controles:lblNormal ID="lblTitulo" runat="server" CssClass="TituloPanel">Ajustar Impuestos</vis1Controles:lblNormal></td>
                                </tr>
                                <tr>
                                    <td colspan="6" style="height: 12px"></td>
                                </tr>
                                <tr>
                                    <td>
                                        <vis1Controles:lblNormal ID="lblImpuestoAnterior" runat="server" CssClass="Etiqueta">Impuesto Anterior:</vis1Controles:lblNormal></td>
                                    <td>
                                        <vis1Controles:cboNormal runat="server" ID="cboImpuestoAnterior" Width="80px" AutoPostBack="true" plPermitirVacio="false"></vis1Controles:cboNormal>
                                    </td>
                                    <td>
                                        <vis1Controles:lblNormal ID="lblImpuestoNuevo" runat="server" CssClass="Etiqueta">Impuesto Nuevo:</vis1Controles:lblNormal></td>
                                    <td>
                                        <vis1Controles:cboNormal runat="server" ID="cboImpuestoNuevo" Width="80px" AutoPostBack="true" plPermitirVacio="false"></vis1Controles:cboNormal>
                                    </td>
                                    <td colspan="2"></td>
                                </tr>
                                <tr>
                                    <td colspan="6" style="height: 16px"></td>
                                </tr>
                                <tr>
                                    <td colspan="6">
                                        <div>
                                            <asp:GridView ID="grdImpuestos" runat="server" EmptyDataText="No hay registros coincidentes"
                                                PageSize="12"  AutoGenerateColumns="False" DataKeyNames="cod_imp,por_imp,mon_imp,mon_net">
                                                <Columns>
                                                    <asp:CommandField ButtonType="Button" SelectText="" ShowSelectButton="True">
                                                        <ItemStyle Height="22px" HorizontalAlign="Center" VerticalAlign="Middle" Width="22px" />
                                                        <ControlStyle CssClass="boton-seleccionar" />
                                                        <HeaderStyle Width="24px"/>
                                                    </asp:CommandField>
                                                    <asp:TemplateField HeaderText="Anterior">
                                                        <ItemTemplate>
                                                            <div style="padding-left: 0.2em; text-align: left; overflow: hidden;">
                                                                <%# 
														            Strings.Trim(Eval("Cod_Imp_Anterior"))
                                                                %>
                                                            </div>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="80px"/>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Nuevo">
                                                        <ItemTemplate>
                                                            <vis1Controles:txtNormal ID="txtCod_Imp" runat="server" Enabled="false" Visible="false" Text='<%# Eval("cod_imp") %>' ></vis1Controles:txtNormal>
                                                            <div style="padding-left: 0.2em; text-align: left; overflow: hidden;">
                                                                <%# 
														            Strings.Trim(Eval("cod_imp"))
                                                                %>
                                                            </div>
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="80px"/>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Monto">
                                                        <ItemTemplate>
                                                            <vis2Controles:txtNumero ID="txtMonto" runat="server" AutoCompleteType="Disabled" AutoPostBack="false"
                                                                CssClass="CajasTextoNumero" pbValor='<%# Eval("mon_net") %>' plAceptarDecimales="true"
                                                                plAceptarNegativos="false" plSeleccionarAlObtenerFoco="true" plUsarSeparadorMiles="true"
                                                                pnNumeroDecimales="2" ReadOnly="true" Width="100px" Enabled="false" Visible="false">
                                                            </vis2Controles:txtNumero>
                                                            <div style="padding-right: 0.2em; text-align: right; font-size: 10pt;">
                                                                <%# 
													                goServicios.mObtenerFormatoCadena(Eval("mon_net"), _
													                goServicios.enuOpcionesRedondeo.KN_RedondeoPuntoMedio, _
													                Me.pnDecimalesMonto)
                                                                %>
                                                            </div>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="100px" />
                                                        <HeaderStyle Width="100px"/>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="% Imp.">
                                                        <ItemTemplate>
                                                            <vis2Controles:txtNumero ID="txtPorImpuesto" runat="server" AutoCompleteType="none" AutoPostBack="false"
                                                                CssClass="CajasTextoNumero" pbValor='<%# Eval("por_imp") %>' plAceptarDecimales="true"
                                                                plAceptarNegativos="false" plSeleccionarAlObtenerFoco="true" plUsarSeparadorMiles="true"
                                                                pnNumeroDecimales="2" ReadOnly="true" Enabled="false" Width="60px" Visible="false">
                                                            </vis2Controles:txtNumero>
                                                            <div style="padding-right: 0.2em; text-align: right; font-size: 10pt;">
                                                                <%# 
													                goServicios.mObtenerFormatoCadena(Eval("por_imp"), _
													                goServicios.enuOpcionesRedondeo.KN_RedondeoPuntoMedio, _
													                Me.pnDecimalesMonto)
                                                                %>
                                                            </div>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="60px" />
                                                        <HeaderStyle Width="60px"/>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Impuesto">
                                                        <ItemTemplate>
                                                            <vis2Controles:txtNumero ID="txtMontoImpuesto" runat="server" AutoCompleteType="none" AutoPostBack="false"
                                                                CssClass="CajasTextoNumero" pbValor='<%# Eval("mon_imp") %>' plAceptarDecimales="true"
                                                                plAceptarNegativos="false" plSeleccionarAlObtenerFoco="true" plUsarSeparadorMiles="true"
                                                                pnNumeroDecimales="2" ReadOnly="true" Enabled="false" Width="100px" Visible="false">
                                                            </vis2Controles:txtNumero>
                                                            <div style="padding-right: 0.2em; text-align: right; font-size: 10pt;">
                                                                <%# 
													                goServicios.mObtenerFormatoCadena(Eval("mon_imp"), _
													                goServicios.enuOpcionesRedondeo.KN_RedondeoPuntoMedio, _
													                Me.pnDecimalesMonto)
                                                                %>
                                                            </div>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="100px" />
                                                        <HeaderStyle Width="100px"/>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <AlternatingRowStyle CssClass="FilaAlternaGrid" />
                                                <RowStyle CssClass="FilaGrid" />
                                                <PagerStyle CssClass="BarraPaginacionGrid" />
                                                <HeaderStyle CssClass="EncabezadoGrid" />
                                                <SelectedRowStyle CssClass="FilaSeleccionadaGrid" />
                                                <EmptyDataRowStyle CssClass="EncabezadoGrid" />
                                            </asp:GridView>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" style="height: 16px"></td>
                                </tr>
                                <tr>
                                    <td colspan="3"></td>
                                    <td colspan="1">
                                        <vis1Controles:lblNormal ID="lblNeto" runat="server" CssClass="Etiqueta">Sub Total:</vis1Controles:lblNormal></td>
                                    <td colspan="2">
                                        <vis2Controles:txtNumero ID="txtNeto" runat="server" CssClass="CajasTextoNumero" plAceptarDecimales="True" pnNumeroDecimales="2" Width="100px" AutoCompleteType="Disabled" ReadOnly="True" Enabled="false">0.00</vis2Controles:txtNumero></td>
                                </tr>
                                <tr>
                                    <td colspan="6" style="height: 4px"></td>
                                </tr>
                                <tr>
                                    <td colspan="3" style="height: 8px"></td>
                                    <td colspan="1">
                                        <vis1Controles:lblNormal ID="lblImpuesto" runat="server" CssClass="Etiqueta">Impuesto:</vis1Controles:lblNormal></td>
                                    <td colspan="2">
                                        <vis2Controles:txtNumero ID="txtImpuesto" runat="server" CssClass="CajasTextoNumero" plAceptarDecimales="True" pnNumeroDecimales="2" Width="100px" ReadOnly="True" Enabled="false">0.00</vis2Controles:txtNumero>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" style="height: 4px"></td>
                                </tr>
                                <tr>
                                    <td colspan="3"></td>
                                    <td colspan="1">
                                        <vis1Controles:lblNormal ID="lblOtros" runat="server" CssClass="Etiqueta">Otros:</vis1Controles:lblNormal></td>
                                    <td colspan="1">
                                        <vis2Controles:txtNumero ID="txtOtros" runat="server" CssClass="CajasTextoNumero" plAceptarDecimales="True" pnNumeroDecimales="2" Width="100px" ReadOnly="True" Enabled="false">0.00</vis2Controles:txtNumero>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" style="height: 4px"></td>
                                </tr>
                                <tr>
                                    <td colspan="3"></td>
                                    <td colspan="1">
                                        <vis1Controles:lblNormal ID="lblTotal" runat="server" CssClass="Etiqueta">Total:</vis1Controles:lblNormal></td>
                                    <td colspan="1">
                                        <vis2Controles:txtNumero ID="txtTotal" runat="server" CssClass="CajasTextoNumero" plAceptarDecimales="True" pnNumeroDecimales="2" Width="100px" ReadOnly="True" Enabled="false">0.00</vis2Controles:txtNumero>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" style="height: 8px"></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div class="divBotoneraComplemento">
						<vis1Controles:cmdNormal ID="cmdAceptar" runat="server" Text="Aceptar" CssClass="BotonAceptar" />
						<vis1Controles:cmdNormal ID="cmdCancelar" runat="server" CssClass="BotonCancelar" Text="Cancelar" />
                    </div>

                    
                    <vis3Controles:wbcAdministradorMensajeModal ID="wbcAdministradorMensajeModal" runat="server" />
                    <vis3Controles:wbcAdministradorVentanaModal ID="WbcAdministradorVentanaModal" runat="server" />
                    <vis3Controles:pnlVentanaModal ID="PnlVentanaModalOperacion" runat="server"
                        pcEstiloBotonCerrar="BotonCerrarVentanaModal" pcEstiloFondo="FondoVentanaModal"
                        pcEstiloMarco="MarcoVentanaModal" pcTextoBotonCerrar="Cerrar" plMostrarBotonCerrar="false" />
                    <vis3Controles:pnlMensajeModal ID="PnlMensajeModalOperacion" runat="server"
                        pcEstiloContenido="ContenidoMensajeModal" pcEstiloFondo="FondoVentanaModal"
                        pcEstiloTitulo="TituloMensajeModal" pcEstiloVentana="MarcoMensajeModal" />

    	        </ContentTemplate>
			</asp:UpdatePanel>

			<asp:UpdateProgress ID="uprProcesando" runat="server" AssociatedUpdatePanelID="udpActualizaciones" DisplayAfter="250">
				<ProgressTemplate>
					<div class="FondoVentanaModal" ></div>
					<div class='divProcesando'></div>
				</ProgressTemplate>
			</asp:UpdateProgress>
		</div>

	</form>
</body>
</html>
