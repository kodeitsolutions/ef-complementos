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
						<td>
                            <label for="lblFecha"  class="Filtro" >Fecha: </label>
						</td>
						<td>
                            <vis2Controles:txtFecha ID="txtFecha" runat="server" style="padding: 0px;margin: 0px;font-size: 13px;" CssClass="CajasTextoFecha" AutoPostBack="false" pdValor="2018-01-01">01/01/2018</vis2Controles:txtFecha>
						</td>
                    </tr>
                    <br /><br />
                    <tr>
                        <td colspan="4" style="height: 10px;"></td>                                
                    </tr>
                    <tr>
                        <td style="vertical-align:top;">
                            <vis1Controles:lblNormal runat="server" ID="lblComentCons" Text="Comentario Consumo:" CssClass="Etiqueta"></vis1Controles:lblNormal>
                        </td>
                        <td>
                            <vis1Controles:txtNormal runat="server" ID="TxtComentCons" CssClass="CajasTexto" Width="500" Enabled="True" TextMode="MultiLine"></vis1Controles:txtNormal>
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
                        <td colspan="2" style="height: 2px;"></td>
                    </tr>--%>
                    <tr>
                        <td>
                            <vis1Controles:lblNormal runat="server" ID="lblTotalConsumido"
                                CssClass="Etiqueta" Text="Total Consumido:" Width="280px"></vis1Controles:lblNormal>
                        </td>                           
                        <td>
                            <vis2Controles:txtNumero runat="server" ID="txtTotalConsumido"
                                CssClass="CajasTextoNumero" Width="110px" plAceptarDecimales="True" plAceptarNegativos="False"
                                plLimitarValores="False" plPermitirComillas="False" plPermitirParentesisAngular="False" 
                                plSeleccionarAlObtenerFoco="True" plUsarSeparadorMiles="True" AutoPostBack="true" Enabled="false"></vis2Controles:txtNumero> 
                        </td>
                    </tr>
                    <br /><br /><br />
                    <tr>
                        <td style="vertical-align:top;"><vis1Controles:lblNormal runat="server" ID="lblComentComentObt" Text="Comentario Obtenido:" CssClass="Etiqueta"></vis1Controles:lblNormal></td>
                        <td>
                            <vis1Controles:txtNormal runat="server" ID="TxtComentObt" CssClass="CajasTexto" Width="500" Enabled="True" TextMode="MultiLine"></vis1Controles:txtNormal>
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
                        <td>
                            <vis1Controles:lblNormal runat="server" ID="lblTotalObtenido"
                                CssClass="Etiqueta" Text="Total Obtenido:" Width="280px"></vis1Controles:lblNormal>
                        </td>                           
                        <td>
                            <vis2Controles:txtNumero runat="server" ID="txtTotalObtenido"
                                CssClass="CajasTextoNumero" Width="110px" plAceptarDecimales="True" plAceptarNegativos="False"
                                plLimitarValores="False" plPermitirComillas="False" plPermitirParentesisAngular="False" 
                                plSeleccionarAlObtenerFoco="True" plUsarSeparadorMiles="True" AutoPostBack="true" Enabled="false"></vis2Controles:txtNumero> 
                        </td>
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

            //loGenerador = (function () {

            //    var mValidarDepartamento = function mValidarArticulo(loArticulo) {

            //        var jqFila = loArticulo.closest('table').closest('tr');

            //        //var loSeccion = jqFila.find('td:nth-child(3) .CajasTexto');

            //        var lcArticulo = loArticulo.val();
            //        var lcRenglon = loArticulo.closest('table').closest('tr').find('td:nth-child(2)').text().trim();

            //        if (lcArticulo.trim() === '') {
            //            loArticulo.val('');
            //            //loSeccion.val('');
            //            return;
            //        }

            //        var loData = {
            //            lcArticulo: lcArticulo
            //        };

            //        var mOk = function mOk(data, textStatus, jqXHR) {

            //            if (!data.llEsValido) {

            //                window.poMensajes.mMostrarMensajeNoModal("Departamento no Válido",
            //                    'El departamento "' + lcArticulo + '" no es válido o está inactivo. Renglón N° ' + lcRenglon + '.', 'a', 150, 550, 5);
            //                loArticulo.val('');
            //                //loSeccion.val('');
            //                jqFila.find('.BotonSeleccion').trigger('click');
            //            };
            //            //} else {
            //            //    mValidarSeccion(loSeccion);
            //            //};

            //        };

            //        jQuery.ajax('CGS_frmMaquila.aspx/mValidarArticulo?UserID=' + window.pcUserId, {
            //            type: 'POST',
            //            data: jQuery.toJSON(loData),
            //            contentType: 'application/json; charset=utf-8',
            //            dataType: 'json',
            //            success: mOk,
            //            error: goFactory.poAjax.mErrorAjax
            //        });

            //    }

            //    var mEnlazarControles = function mEnlazarControles() {
            //        /// <summary>Enlaza los controles del grid con las funciones de validación.</summary>
                    
            //        var jqFilas = jQuery('#grdConsumido_Renglones').find('tr.FilaGrid,tr.FilaAlternaGrid,tr.FilaSeleccionadaGrid');

            //        //Validación de Departamentos
            //        jqFilas.find(' td:nth-child(1) .CajasTexto').on('change', function (e) {

            //            mValidarArticulo(jQuery(this));

            //            return;
            //        });                   
                    
            //        // Boton de selección
            //        var jqBotones = jqFilas.find(' td:nth-child(1) .BotonSeleccion');
            //        jqBotones.on('click', function (e) {
            //            e.preventDefault();

            //            var jqFila = jQuery('#grdConsumido_Renglones .FilaSeleccionadaGrid');

            //            if (jqFila.is(':nth-child(2n+1)')) {
            //                jqFila.addClass('FilaGrid').removeClass('FilaSeleccionadaGrid');
            //            } else {
            //                jqFila.addClass('FilaAlternaGrid').removeClass('FilaSeleccionadaGrid');
            //            }

            //            jQuery(this).closest('tr').addClass('FilaSeleccionadaGrid').removeClass('FilaGrid').removeClass('FilaAlternaGrid')
            //            return false;

            //        });

            //    };

            //    return {
            //        mEnlazarControles: mEnlazarControles
            //    }
            //})();

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
            window.resizeTo(800, 720);
        </script>

    </form>

</body>
</html>
