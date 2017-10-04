using System;
using System.Collections.Generic;
using System.Linq;

using OperationListEntity = POCDriverApp.Entity.OperationList;

namespace POCDriverApp.Entity
{
    public class OperationListEx
    {
        public OperationListEx(OperationListEntity operationList)
        {
            _operationList = operationList;
        }

        public OperationListEx()
        {
            _operationList = new OperationListEntity();
        }

        private readonly OperationListEntity _operationList;
        public OperationListStop[] Stop
        {
            get { return _operationList.Stop; }
            set { _operationList.Stop = value; }
        }

        public string VersionNumber { get { return _operationList.VersionNumber; } set { _operationList.VersionNumber = value; } }
        public string SessionId { get { return _operationList.SessionId; } }
        public string OperationListId { get { return _operationList.OperationListId; } }
        public string CaptureEquipmentId { get { return _operationList.CaptureEquipmentId; } }
        public DateTime LastChanged { get { return _operationList.LastChanged; } }
        public string CorrelationId { get { return _operationList.CorrelationId; } }
        public string[] DeletedOrders { get { return _operationList.DeletedOrders; } }
        public ChangeStatus LastUpdateChangeStatus { get; set; }

        private bool resetSequence;

        public static explicit operator OperationListEntity(OperationListEx d)  // implicit digit to byte conversion operator
        {
            return d._operationList;
        }


        /// <summary>
        /// Retrieve stop based on stop-id
        /// </summary>
        /// <param name="stopId"></param>
        /// <returns>operations for one stop</returns>
        public OperationListStop GetStop(String stopId)
        {
            OperationListStop result = null;
            if ((Stop != null) && !String.IsNullOrEmpty(stopId))
            {
                result = (from n in Stop
                          where n.StopInformation != null && n.StopInformation.StopId == stopId
                          select n).SingleOrDefault<OperationListStop>();
            }

            return result;
        }

        /// <summary>
        /// Retrieve stop containing a given operation
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns>operations for one stop</returns>
        public OperationListStop GetStopByOperationId(String operationId)
        {
            OperationListStop result = null;
            if (Stop != null)
            {
                result = (from n in Stop
                          where n.StopInformation != null && n.PlannedOperation[0] != null && n.PlannedOperation[0].OperationId == operationId
                          select n).SingleOrDefault<OperationListStop>();
            }

            return result;
        }

        /// <summary>
        /// Retrieve the selected planned operations for stop
        /// </summary>
        /// <param name="stopId"></param>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public PlannedOperationType GetOperation(String stopId, string operationId)
        {
            var operationListForStop = GetStop(stopId);
            if (operationListForStop == null)
            {
                if (!String.IsNullOrEmpty(operationId))
                {
                   return GetOperation(operationId);
                }
                else
                    return null;
            }

            var operation = (from n in operationListForStop.PlannedOperation
                             where n.OperationId == operationId
                             select n).SingleOrDefault<PlannedOperationType>();
            return operation;
        }

        public PlannedOperationType GetOperation(string operationId)
        {
            var stop = GetStopByOperationId(operationId);
            if (stop != null)
                return stop.PlannedOperation[0];
            return null;
        }


        // Amphora has empty stop-ids, and only one operation per stop
        // For that reason we put operation-id into stop-id field, so we can 
        // have a unique id per stop
        public void AddMissingStopIdsForAmphora()
        {
            try
            {
                if (Stop != null)
                {
                    foreach (var stop in Stop)
                    {
                        if (string.IsNullOrEmpty(stop.StopInformation.StopId))
                        {
                            stop.StopInformation.StopId = stop.PlannedOperation[0].OperationId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               
            }
        }

        // Add a stop to operationlist if it does not exist already if we have received a partial update
        public void AddMissingStops(OperationListEx existingOperationList)
        {
            try
            {
                if (existingOperationList != null)
                {
                    var existingStops = Stop.ToList();

                    // Copy operations from existing OL to new OL
                    foreach (var stop in existingOperationList.Stop)
                    {
                        if (stop.PlannedOperation != null && stop.PlannedOperation[0] != null)
                        {
                            var existingStop = GetStopByOperationId(stop.PlannedOperation[0].OperationId);
                            // If stop not exists, we add it to list of stops
                            if (existingStop == null)
                            {   
                                existingStops.Add(stop);

                                // Add status for existing order(which by definition is confirmed already)
                                if (ModelUser.UserProfile.TmsAffiliation == TmsAffiliation.Alystra)
                                {
                                    var orderId = stop.PlannedOperation[0].OrderID;
                                    SetOrderConfirmationStatus(orderId, existingOperationList.GetOrderConfirmationStatus(orderId));
                                }
                            }
                        }
                    }

                    Stop = existingStops.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "OperationList.AddStopIfMissing");
            }
        }

        public void AddMissingStopsAlystra(OperationListEx existingOperationList)
        {
            try
            {
                if (existingOperationList != null)
                {
                    //var existingStops = Stop.ToList();
                    var existingStops = new List<OperationListStop>();

                   
                    // Copy operations from existing OL to new OL
                    foreach (var stop in existingOperationList.Stop)
                    {
                        if (stop!=null && stop.PlannedOperation != null && stop.PlannedOperation[0] != null)
                        {
                            var existingStop = GetStopByOperationId(stop.PlannedOperation[0].OperationId);
                            // If stop not exists, we add it to list of stops
                            if (existingStop == null)
                            {                                
                                existingStops.Add(stop);

                                // Add status for existing order(which by definition is confirmed already)
                                if (ModelUser.UserProfile.TmsAffiliation == TmsAffiliation.Alystra)
                                {
                                    var orderId = stop.PlannedOperation[0].OrderID;
                                    SetOrderConfirmationStatus(orderId, existingOperationList.GetOrderConfirmationStatus(orderId));
                                }
                            }
                        }
                    }

                    foreach (var item in Stop)
                    {
                        if (item != null && item.StopInformation != null)
                        {
                            if (ModelOperationList.OperationList != null && ModelOperationList.OperationList.Stop != null)
                            {
                                var newStop = ModelOperationList.OperationList.Stops(stop => stop.Status != OperationStatus.Canceled).Where(s => s.StopInformation.StopId == item.StopInformation.StopId
                                    && s.StopInformation.StopSequence == item.StopInformation.StopSequence).ToList();
                                
                                if (newStop.Count == 0)
                                {
                                    resetSequence = true;
                                    existingStops = ResetExistingStops(existingStops);
                                }                                
                            }
                            existingStops.Add(item);
                        }
                    }

                    existingStops = existingStops.OrderBy(s => int.Parse(s.StopInformation.StopSequence)).ToList();

                    Stop = existingStops.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "OperationList.AddStopIfMissingAlystra");
            }
        }

        private List<OperationListStop> ResetExistingStops(List<OperationListStop> existingStops)
        {
            if (resetSequence)
            {                
                existingStops = existingStops.OrderBy(s => int.Parse(s.StopInformation.StopSequence)).ToList();
               
                foreach (var stop in existingStops)
                {
                    if (stop.PlannedOperation != null && stop.PlannedOperation[0] != null)
                    {
                        stop.StopInformation.StopSequence = (existingStops.IndexOf(stop) + 1).ToString();
                    }
                }
                resetSequence = false;
            }
            return existingStops;
        }

        // Retrieve all distinct ordernumbers in OL
        public List<string> GetOrderNumbers()
        {
            var result = new List<string>();

            //Iterate for all operations in current stop
            if (Stop != null)
            {
                foreach (var stop in Stop)
                {
                    if (stop.PlannedOperation != null)
                    {
                        var orderNumber = stop.PlannedOperation[0].OrderID;

                        if (string.IsNullOrEmpty(orderNumber) == false && result.Contains(orderNumber) == false)
                        {
                            result.Add(orderNumber);
                        }
                    }
                }
            }

            return result;
        }

        // Return list of stops that qualify to a given condition
        public List<OperationListStop> Stops(Func<OperationListStop, bool> func)
        {
            return (from stop in Stop where stop != null && func(stop) select stop).ToList();
        }

        // Return list of operations that qualify to a given condition
        public List<PlannedOperationType> Operations(Func<PlannedOperationType, bool> func)
        {
            var validStops = Stop.Where(stop => stop.PlannedOperation != null).ToList();
            return (from stop in validStops from operation in stop.PlannedOperation where stop.PlannedOperation != null && operation != null && func(operation) select operation).ToList();
        }

        /// <summary>
        /// Update status of an order as following:
        ///     New: Order has only new operations
        ///     Confirmed: Order was confirmed earlier (in earlier update) or this is the first OL we receive and no operations are New
        ///     Updated: Has operations that are not finished 
        /// </summary>
        public void UpdateOrderConfirmationStatus(string orderNumber)
        {
            string confirmationStatus;

            // All operations cancelled
            if (Operations(o => o.OrderID == orderNumber && o.Status != OperationStatus.Canceled).Any() == false)
                confirmationStatus = ConfirmationStatus.Canceled;
            // All operations new
            else if (Operations(o => o.OrderID == orderNumber && o.Status != OperationStatus.New).Any() == false)
                confirmationStatus = ConfirmationStatus.New;
            // All operations finished
            else if (ModelOperationList.OperationList == null && Operations(o => o.OrderID == orderNumber && o.Status == OperationStatus.New).Any() == false ||
                Operations(o => o.OrderID == orderNumber && o.Status != OperationStatus.Finished).Any() == false)
            {
                confirmationStatus = ConfirmationStatus.Confirmed;
            }
            // SPECIAL CASE: Exclusively Finished and New operations together are treated as NEW
            else if (Operations(o => o.OrderID == orderNumber && (o.Status != OperationStatus.New && o.Status != OperationStatus.Finished)).Any() == false)
            {
                confirmationStatus = ConfirmationStatus.New;
            }
            else
                confirmationStatus = ConfirmationStatus.Updated;

            SetOrderConfirmationStatus(orderNumber, confirmationStatus);
        }

        private Dictionary<string, string> _orderConfirmationStatus;

        private Dictionary<string, string> OrderConfirmationStatus
        {
            get { return _orderConfirmationStatus ?? (_orderConfirmationStatus = new Dictionary<string, string>()); }
        }

        public string GetOrderConfirmationStatus(string orderId)
        {
            string status;
            OrderConfirmationStatus.TryGetValue(orderId, out status);
            return status;
        }

        public void SetOrderConfirmationStatus(string orderNumber, string status)
        {
            OrderConfirmationStatus[orderNumber] = status;
        }


        public bool IsOrderActive(string orderNumber)
        {
            // If we have started operations, then the order is active
            if (Operations(o => o.OrderID == orderNumber && o.Status == OperationStatus.Started).Any())
                return true;

            return false;
        }

        public bool IsOrderNew(string orderNumber)
        {
            // If we have started operations, then the order is active
            if (Operations(o => o.OrderID == orderNumber && o.Status != OperationStatus.New).Any())
                return false;

            return true;
        }

        public string GetOrderOperationStatus(string orderNumber)
        {
            //if no stop without cancelled staus, then order is cancelled
            if (Operations(o => o.OrderID == orderNumber && o.Status != OperationStatus.Canceled).Any() == false)
                return OperationStatus.Canceled;

            //if no stop with Finish status then order treated as New status
            if (Operations(o => o.OrderID == orderNumber && o.Status == OperationStatus.Finished).Any() == false)
                return OperationStatus.New;

            //if stop of order is started and some is finished/cancelled then it is order is started...
            if (Operations(o => o.OrderID == orderNumber && (o.Status == OperationStatus.Started)).Any() && Operations(o => o.OrderID == orderNumber &&
                (o.Status == OperationStatus.Finished || o.Status == OperationStatus.Canceled)).Any())
                return OperationStatus.Started;


            return OperationStatus.Finished;
        }

        public enum ChangeStatus
        {
            
            Update,     // All orders to confirm are updates
            New,        // Have only new orders
            Mixed,      // Have both new and updated orders
            Canceled,    // Have only canceled orders
            Confirmed,   // Have only confirmed orders
            UpdateOnNew, // Already received order that needs confirmation, and now received an update to that order
            Accepted  //Defect 1023 - Accepted status added when the PDA gets rebooted and the last status is in a hung state or the Log Out event for the previous session had not been sent out to AMPHORA due to a hard boot
        }


        public ChangeStatus ListConfirmationStatus
        {
            get
            {
                if (ModelUser.UserProfile.TmsAffiliation == TmsAffiliation.Alystra)
                {
                    var orderNumbers = GetOrderNumbers();

                    var statuses = orderNumbers.Select(orderNumber => GetOrderConfirmationStatus(orderNumber)).ToArray();

                    

                    // Only new orders
                    if (statuses.Any(status => status == ConfirmationStatus.New))
                        return ChangeStatus.New;

                    // No new orders
                    if (statuses.Any(status => status == ConfirmationStatus.New)== false)
                        return ChangeStatus.Update;

                }

                if (ModelUser.UserProfile.TmsAffiliation == TmsAffiliation.Amphora)
                {
                    var operations = Operations(o => o.Status == OperationStatus.New || o.Status == OperationStatus.Canceled || o.Status == OperationStatus.UpdateOnNew);

                    // Only new operations
                    if (operations.All(o => o.Status == OperationStatus.New))
                        return ChangeStatus.New;

                    //All Cancelled
                    if (operations.All(o => o.Status == OperationStatus.Canceled || o.Status == OperationStatus.Cancelled))
                        return ChangeStatus.Canceled;

                    // No new operations
                    if (operations.Any(o => o.Status == OperationStatus.New || o.Status == OperationStatus.Canceled))
                        return ChangeStatus.Mixed;

                    // No new operations
                    if (operations.Any(o => o.Status == OperationStatus.New) == false)
                        return ChangeStatus.Update;
                }

                return ChangeStatus.Mixed;
            }
        }

        public ChangeStatus OperationListOrdersUpdateConfirmationStatus(OperationListEx prevOperationList)
        {
            var orderNumbers = GetOrderNumbers();

            var statuses = orderNumbers.Select(orderNumber => GetOrderConfirmationStatus(orderNumber)).ToArray();

            // Only canceled orders
            if (statuses.All(status => status == ConfirmationStatus.Canceled))
                return ChangeStatus.Canceled;

            // Only confirmed orders
            if (statuses.All(status => status == ConfirmationStatus.Confirmed))
                return ChangeStatus.Confirmed;
            

            // Only new orders
            if (statuses.All(status => status != ConfirmationStatus.New && status != ConfirmationStatus.Confirmed))
            {
                if (prevOperationList != null && prevOperationList.Stop != null &&
                    prevOperationList.Stop.Length > 0)
                {
                    var prevOrderNumbers = prevOperationList.GetOrderNumbers();
                    if (orderNumbers.All(prevOrderNumbers.Contains))
                        return ChangeStatus.UpdateOnNew;
                }

                return ChangeStatus.New;
            }

            // No new orders
            if (statuses.Any(status => status == ConfirmationStatus.New))
            {
                if (prevOperationList != null && prevOperationList.Stop != null &&
                    prevOperationList.Stop.Length > 0)
                {
                    var prevOrderNumbers = prevOperationList.GetOrderNumbers();
                    if (orderNumbers.All(prevOrderNumbers.Contains))
                        return ChangeStatus.UpdateOnNew;
                }
                return ChangeStatus.New;
            }

            return ChangeStatus.Mixed;
        }

        public ChangeStatus OperationListOperationsConfirmationStatus(OperationListEx prevOperationList)
        {
            var operations = GetOperations();

            var statuses = operations.Select(operation => operation.Status).ToArray();

            // Only canceled operations
            if (statuses.All(status => status == ConfirmationStatus.Canceled))
                return ChangeStatus.Canceled;

            // Only confirmed operations
            if (statuses.All(status => status == ConfirmationStatus.Confirmed))
                return ChangeStatus.Confirmed;

            
            // Defect 1023 - Only accepted operations
            if (statuses.All(status => status == ConfirmationStatus.Accepted))
                return ChangeStatus.Accepted;


            // No new operations
            if (statuses.Any(status => status == ConfirmationStatus.New) == false)
                return ChangeStatus.Update;

            // Only new orders
            if (statuses.All(status => status == ConfirmationStatus.New))
            {
                if (prevOperationList != null && prevOperationList.Stop != null && prevOperationList.Stop.Length > 0)
                {
                    var prevOperations = prevOperationList.GetOperations();
                    if (operations.Any(o => prevOperations.Any(po => po.OperationId == o.OperationId)))
                        return ChangeStatus.UpdateOnNew;
                }

                return ChangeStatus.New;
            }

            return ChangeStatus.Mixed;
        }

        public int AcceptCount { get; set; }
        public int RejectCount { get; set; }
        public List<string> UpdateOnNewOrderNumbers { get; set; }
        public List<PlannedOperationType> UpdateOnNewOperations { get; set; }


        public OperationListEx CreateCopy()
        {
            var innerObject = BaseActionCommands.GetCopyOfObject<OperationListEntity>(_operationList);
            return new OperationListEx(innerObject);
        }

        public bool IsStopSignatureRequired(string stopId)
        {
            if (string.IsNullOrEmpty(stopId) == false)
            {
                OperationListStop operationListStop = GetStop(stopId);
                if (operationListStop != null && operationListStop.PlannedOperation != null && operationListStop.PlannedOperation[0] != null)
                {
                    return operationListStop.PlannedOperation[0].OperationSignatureRequired;
                }
            }
            return true;
        }


        public void DeleteCanceledStops()
        {
            try
            {
                Stop = Stops(s => s.PlannedOperation != null && s.PlannedOperation[0].Status != OperationStatus.Canceled).ToArray();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "OperalistEx.DeleteCanceledStops");
            }
        }

        public void StartUpdatedStops()
        {
            try
            {
                foreach (var stop in Stop)
                {
                    if (stop.PlannedOperation != null && stop.PlannedOperation[0].Status == ConfirmationStatus.Updated)
                        stop.PlannedOperation[0].Status = OperationStatus.Started;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "OperalistEx.StartUpdatedStops");
            }
        }

        public bool HasUnconfirmedOrders
        {
            get
            {
                var orderNumbers = GetOrderNumbers();
                return orderNumbers.Select(orderNumber => GetOrderConfirmationStatus(orderNumber)).Any(
                    orderConfirmationStatus => orderConfirmationStatus == ConfirmationStatus.New ||
                                               orderConfirmationStatus == ConfirmationStatus.Updated ||
                                               orderConfirmationStatus == ConfirmationStatus.Canceled);
            }
        }

        public bool HasUnconfirmedOperations
        {
            get
            {
                var operations = GetOperations();
                return operations.Any(operation => operation.Status == ConfirmationStatus.New || operation.Status == ConfirmationStatus.Updated || operation.Status == ConfirmationStatus.Canceled);
            }
        }

        public List<PlannedOperationType> GetOperations()
        {
            return Stop.Select(stop => stop.PlannedOperation[0]).ToList();
        }
    }

    public enum OperationListConfirmStatus
    {
        IsUpdated,  // All operations to confirm are updates
        IsNew,      // All operations to confirm are new
        IsMixed     // Orders to confirm are mix of both updated and new 
    }
}
