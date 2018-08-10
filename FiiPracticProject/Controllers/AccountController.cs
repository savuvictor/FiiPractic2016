using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FiiPracticProject.DataStore;
using FiiPracticProject.Models;

namespace FiiPracticProject.Controllers
{
    [RoutePrefix("api/account")]

    public class AccountController : ApiController
    {
        [HttpPost]
        [Route("addAccount")]
        public IHttpActionResult AddAccount(AccountModel account)
        {
            try
            {
                if (account == null) return BadRequest("You cannot add an empty account!");
                var allAccounts = DataStoreUtil.ReadModels() ?? new List<AccountModel>();

                if (allAccounts.Contains(allAccounts.FirstOrDefault(n => n.Name.Contains(account.Name))))
                    return BadRequest("A user with this name already exists!");
                // We dont allow multiple users with same name because we will have conflicts in update/delete/delete_sensor

                allAccounts.Add(account);

                DataStoreUtil.SaveModels(allAccounts);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("deleteAccount")]
        public IHttpActionResult DeleteAccout(string account)
        {
            try
            {
                var allAccounts = DataStoreUtil.ReadModels();

                var currentAccount = allAccounts.FirstOrDefault(n => n.Name.Equals(account));

                if (currentAccount == null) return NotFound();
                allAccounts.RemoveAll(n => n.Name == currentAccount.Name);

                DataStoreUtil.SaveModels(allAccounts);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("updateAccount")]
        public IHttpActionResult UpdateAccount(string account, AccountModel _updated)
        {
            try
            {
                var allAccounts = DataStoreUtil.ReadModels();

                var currentAccount = allAccounts.FirstOrDefault(a => a.Name.Equals(account));

                if (currentAccount == null) return NotFound();
                if (_updated == null) return BadRequest("You can't update your account with empty values!");

                currentAccount.Name = _updated.Name;
                currentAccount.Password = _updated.Password;
                currentAccount.Location = _updated.Location;
                currentAccount.Temperature = _updated.Temperature;
                currentAccount.SleepHour = _updated.SleepHour;
                currentAccount.SleepMinute = _updated.SleepMinute;

                DataStoreUtil.SaveModels(allAccounts);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("addSensor")]
        public IHttpActionResult AddSensor(string account, string sensorId)
        {
            try
            {
                var allAccounts = DataStoreUtil.ReadModels();
                var currentAccount = allAccounts.FirstOrDefault(a => a.Name.Equals(account));

                if (currentAccount == null) return NotFound();

                if (currentAccount.Sensors == null)

                    currentAccount.Sensors = new List<string>();
                currentAccount.Sensors.Add(sensorId);
                DataStoreUtil.SaveModels(allAccounts);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("deleteSensor")]
        public IHttpActionResult DeleteSensor(string account, string sensorId)
        {
            try
            {
                var allAccounts = DataStoreUtil.ReadModels();

                var currentAccount = allAccounts.FirstOrDefault(y => y.Name.Equals(account));

                if (currentAccount == null) return NotFound();
                if (currentAccount.Sensors == null) return NotFound();

                var currentSensor = allAccounts.All(s => s.Sensors.Equals(sensorId));

                currentAccount.Sensors.Remove(sensorId);

                DataStoreUtil.SaveModels(allAccounts);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("getSettings")]
        public IHttpActionResult GetSettingsBySensorId(string sensorId)
        {
            try
            {
                var allAccounts = DataStoreUtil.ReadModels();

                var currentSensor = allAccounts.FirstOrDefault(a => a.Sensors.Contains(sensorId));
                if (currentSensor == null) return NotFound();

                if (sensorId.ToLower().Contains("switch"))
                {
                    return BadRequest("This is a switch sensor");
                }
                else
                {
                    return Ok(new
                    {
                        Temperature = currentSensor.Temperature,
                        SleepHour = currentSensor.SleepHour,
                        SleepMinute = currentSensor.SleepMinute,
                    });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
