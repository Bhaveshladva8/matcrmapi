using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class TableColumnUserService : Service<TableColumnUser>, ITableColumnUserService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public TableColumnUserService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<TableColumnUser> CheckInsertOrUpdate(TableColumnUserDto model)
        {
            var tableColumnUserObj = _mapper.Map<TableColumnUser>(model);

            var existingItem = _unitOfWork.TableColumnUserRepository.GetMany(t => t.MasterTableId == tableColumnUserObj.MasterTableId && t.TableColumnId == tableColumnUserObj.TableColumnId && t.UserId == tableColumnUserObj.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertTableColumnUser(tableColumnUserObj);
            }
            else
            {
                existingItem.Priority = tableColumnUserObj.Priority;
                existingItem.IsHide = tableColumnUserObj.IsHide;
                return await UpdateTableColumnUser(existingItem, existingItem.Id);
            }
        }

        public async Task<List<TableColumnUser>> CheckInsertOrUpdateList(List<TableColumnUserDto> modelList)
        {
            List<TableColumnUser> columnUserList = new List<TableColumnUser>();
            List<TableColumnUser> InsertColumnList = new List<TableColumnUser>();
            List<TableColumnUser> UpdateColumnList = new List<TableColumnUser>();
            foreach (var model in modelList)
            {
                var tableColumnUser = _mapper.Map<TableColumnUser>(model);
                var existingItem = _unitOfWork.TableColumnUserRepository.GetMany(t => t.MasterTableId == tableColumnUser.MasterTableId && t.TableColumnId == tableColumnUser.TableColumnId && t.UserId == tableColumnUser.UserId && t.IsDeleted == false).Result.FirstOrDefault();
                try
                {
                    if (existingItem == null)
                    {
                        tableColumnUser.CreatedOn = DateTime.UtcNow;
                        InsertColumnList.Add(tableColumnUser);
                        // var AddUpdate = InsertTableColumnUser1 (obj);
                        // obj.Id = AddUpdate.Id;
                        columnUserList.Add(tableColumnUser);
                    }
                    else
                    {
                        existingItem.Priority = tableColumnUser.Priority;
                        existingItem.IsHide = tableColumnUser.IsHide;
                        UpdateColumnList.Add(existingItem);
                        // var AddUpdate = UpdateTableColumnUser1 (existingItem, existingItem.Id);
                        // obj.Id = AddUpdate.Id;
                        columnUserList.Add(tableColumnUser);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            if (InsertColumnList.Count() > 0)
            {
                await InsertTableColumnUser1(InsertColumnList);
            }
            if (UpdateColumnList.Count() > 0)
            {
                await UpdateTableColumnUser1(UpdateColumnList);
            }
            // await _unitOfWork.CommitAsync ();
            return columnUserList;
        }

        public async Task<TableColumnUser> InsertTableColumnUser(TableColumnUser tableColumnUserObj)
        {
            tableColumnUserObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.TableColumnUserRepository.AddAsync(tableColumnUserObj);
            // await _unitOfWork.BeginTransaction ();
            await _unitOfWork.CommitAsync();

            // await _unitOfWork.EndTransaction ();

            return newItem;
        }

        public async Task<List<TableColumnUser>> InsertTableColumnUser1(List<TableColumnUser> tableColumnUserList)
        {
            // TableColumnUser.CreatedOn = DateTime.UtcNow;
            // var newItem = _unitOfWork.TableColumnUserRepository.AddAsync (TableColumnUser);
            // await _unitOfWork.CommitAsync ();
            var newItem = _unitOfWork.TableColumnUserRepository.AddRange(tableColumnUserList);
            await _unitOfWork.CommitAsync();

            return newItem.Result;
        }

        public async Task<TableColumnUser> UpdateTableColumnUser(TableColumnUser existingItem, long existingId)
        {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            var data = await _unitOfWork.TableColumnUserRepository.UpdateAsync(existingItem, existingId);
            // await _unitOfWork.BeginTransaction ();
            await _unitOfWork.CommitAsync();

            // await _unitOfWork.EndTransaction ();

            // return existingItem.Result;
            return data;
        }

        public async Task<List<TableColumnUser>> UpdateTableColumnUser1(List<TableColumnUser> tableColumnUserList)
        {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            // _unitOfWork.TableColumnUserRepository.UpdateAsync (existingItem, existingId);
            // await _unitOfWork.CommitAsync ();

            // return existingItem.Result;

            var newItem = _unitOfWork.TableColumnUserRepository.UpdateRange(tableColumnUserList);
            await _unitOfWork.CommitAsync();
            return newItem.Result;
        }

        public List<TableColumnUser> GetAll()
        {
            return _unitOfWork.TableColumnUserRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public TableColumnUser GetById(long Id)
        {
            return _unitOfWork.TableColumnUserRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public List<TableColumnUser> GetByUser(int UserId)
        {
            return _unitOfWork.TableColumnUserRepository.GetMany(t => t.UserId == UserId && t.IsDeleted == false).Result.ToList();
        }

        public List<TableColumnUser> GetAllByTable(long MasterTableId, int UserId)
        {
            return _unitOfWork.TableColumnUserRepository.GetMany(t => t.MasterTableId == MasterTableId && t.UserId == UserId && t.IsDeleted == false).Result.ToList();
        }

    }

    public partial interface ITableColumnUserService : IService<TableColumnUser>
    {
        Task<TableColumnUser> CheckInsertOrUpdate(TableColumnUserDto model);
        Task<List<TableColumnUser>> CheckInsertOrUpdateList(List<TableColumnUserDto> modelList);
        List<TableColumnUser> GetAll();
        TableColumnUser GetById(long Id);
        List<TableColumnUser> GetByUser(int UserId);
        List<TableColumnUser> GetAllByTable(long MasterTableId, int UserId);
    }
}