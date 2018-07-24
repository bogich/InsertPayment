using System;
using System.Configuration;
using System.Collections.Generic;
using Gis.Crypto;
using Gis.Infrastructure.PaymentServiceAsync;

namespace Gis.Helpers.HelperPaymentAsync
{
    class HelperPaymentAsync
    {
        /// <summary>
        /// Извещение о принятии к исполнению распоряжения, размещаемое исполнителем
        /// </summary>
        /// <param name="_orgPPAGUID">
        /// Идентификатор зарегистрированной организации
        /// </param>
        /// <param name="_listNotificationOfOrder">
        /// Массив передавамых документов (не более 1000 записей)
        /// </param>
        /// <returns></returns>
        public string SetSupplierNotificationsOfOrderExecution(string _orgPPAGUID, List<dynamic> _listNotificationOfOrder)
        {
            var srvPaymentSrvAsync = new PaymentPortsTypeAsyncClient();
            srvPaymentSrvAsync.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["_login"];
            srvPaymentSrvAsync.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["_pass"];

            var SupplierNotificationOfOrderExecution = new List<importSupplierNotificationsOfOrderExecutionRequestSupplierNotificationOfOrderExecution>();
            foreach (var item in _listNotificationOfOrder)
            {
                SupplierNotificationOfOrderExecution.Add(
                    new importSupplierNotificationsOfOrderExecutionRequestSupplierNotificationOfOrderExecution
                    {
                        TransportGUID = Guid.NewGuid().ToString(),
                        ItemElementName = ItemChoiceType1.PaymentDocumentID,
                        Item = (string)item[0],
                        OrderDate = item[1],
                        Amount = Convert.ToDecimal(item[2].ToString("F"))
                    });
            }

            var reqImpSupNotifOrderExec = new importSupplierNotificationsOfOrderExecutionRequest1
            {
                RequestHeader = new RequestHeader
                {
                    Date = DateTime.Now,
                    MessageGUID = Guid.NewGuid().ToString(),
                    ItemElementName = ItemChoiceType.orgPPAGUID,
                    Item = _orgPPAGUID
                },
                importSupplierNotificationsOfOrderExecutionRequest = new importSupplierNotificationsOfOrderExecutionRequest
                {
                    version = "10.0.1.1",
                    Id = CryptoConsts.CONTAINER_ID,
                    SupplierNotificationOfOrderExecution = SupplierNotificationOfOrderExecution.ToArray()
                }
            };

            var resImpSupNotifOrderExec = srvPaymentSrvAsync.importSupplierNotificationsOfOrderExecution(reqImpSupNotifOrderExec);
            var ackImpSupNotifOrderExec = resImpSupNotifOrderExec.AckRequest.Ack.MessageGUID.ToString();

            return ackImpSupNotifOrderExec;
        }

        /// <summary>
        /// Извещение об аннулировании извещения о принятии к исполнению распоряжения
        /// </summary>
        /// <param name="_OrderID">
        /// Уникальный идентификатор распоряжения для нужд квитирования
        /// </param>
        /// <returns></returns>
        public string SetNotificationsOfOrderExecutionCancellation(string _orgPPAGUID, string _orderID)
        {
            var srvPaymentSrvAsync = new PaymentPortsTypeAsyncClient();
            srvPaymentSrvAsync.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["_login"];
            srvPaymentSrvAsync.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["_pass"];

            var reqNotifOrderExecCancellation = new importNotificationsOfOrderExecutionCancellationRequest1
            {
                RequestHeader = new RequestHeader
                {
                    Date = DateTime.Now,
                    MessageGUID = Guid.NewGuid().ToString(),
                    ItemElementName = ItemChoiceType.orgPPAGUID,
                    Item = _orgPPAGUID
                },
                importNotificationsOfOrderExecutionCancellationRequest = new importNotificationsOfOrderExecutionCancellationRequest
                {
                    version = "10.0.1.1",
                    Id = CryptoConsts.CONTAINER_ID,
                    NotificationOfOrderExecutionCancellation = new NotificationOfOrderExecutionCancellationType[]
                    {
                        new NotificationOfOrderExecutionCancellationType
                        {
                            TransportGUID = Guid.NewGuid().ToString(),
                            CancellationDate = DateTime.Now,
                            Comment = "Сведения загружены ошибочно",
                            OrderID = _orderID
                        }
                    }
                }
            };

            var resNotifOrderExecCancellation = srvPaymentSrvAsync.importNotificationsOfOrderExecutionCancellation(reqNotifOrderExecCancellation);
            var ackNotifOrderExecCancellation = resNotifOrderExecCancellation.AckRequest.Ack.MessageGUID.ToString();

            return ackNotifOrderExecCancellation;
        }

        /// <summary>
        /// Получить статус обработки запроса
        /// </summary>
        /// <param name="_orgPPAGUID">
        /// Идентификатор зарегистрированной организации
        /// </param>
        /// <param name="_ackImpSupNotifOrderExec">
        /// Идентификатор сообщения, присвоенный ГИС ЖКХ
        /// </param>
        /// <returns></returns>
        public getStateResponse P_GetState(string _orgPPAGUID, string _ackImpSupNotifOrderExec)
        {
            var srvPaymentSrvAsync = new PaymentPortsTypeAsyncClient();
            srvPaymentSrvAsync.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["_login"];
            srvPaymentSrvAsync.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["_pass"];

            var reqImpSupNotifOrderExec_GS = new getStateRequest1
            {
                RequestHeader = new RequestHeader
                {
                    Date = DateTime.Now,
                    MessageGUID = Guid.NewGuid().ToString(),
                    ItemElementName = ItemChoiceType.orgPPAGUID,
                    Item = _orgPPAGUID
                },
                getStateRequest = new getStateRequest
                {
                    MessageGUID = _ackImpSupNotifOrderExec
                }
            };

            var resGetState = srvPaymentSrvAsync.getState(reqImpSupNotifOrderExec_GS);

            return resGetState;
        }
    }
}
