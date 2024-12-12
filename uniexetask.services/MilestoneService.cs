using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;
using uniexetask.shared.Models.Request;

namespace uniexetask.services
{
    public class MilestoneService : IMilestoneService
    {
        public IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public MilestoneService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Milestone?> GetMilestoneWithCriteria(int id)
        {
            return await _unitOfWork.Milestones.GetMileStoneWithCriteria(id);
        }

        public async Task<IEnumerable<Milestone>> GetMileStones()
        {
            return await _unitOfWork.Milestones.GetAsync();
        }

        public async Task<IEnumerable<Milestone>> GetMileStonesBySubjectId(int subjectId)
        {
            return await _unitOfWork.Milestones.GetAsync(filter: m => m.SubjectId == subjectId);
        }

        public async Task<Milestone?> GetUndeleteMilestoneWithCriteria(int id)
        {
            return await _unitOfWork.Milestones.GetUndeleteMileStoneWithCriteria(id);
        }

        public async Task<IEnumerable<Milestone>> GetUndeleteMileStonesBySubjectId(int subjectId)
        {
            return await _unitOfWork.Milestones.GetAsync(filter: m => m.SubjectId == subjectId && !m.IsDeleted);
        }

        public async Task<bool> ImportMilestoneWithCriteriaFromExcel(IFormFile excelFile)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var milestoneDictionary = new Dictionary<int, MilestoneWithCriteriaModel>();

                var milestoneCurrent = await _unitOfWork.Milestones.GetAllUndeleteMileStoneAsync();
                foreach (var item in milestoneCurrent)
                {
                    item.IsDeleted = true;
                    _unitOfWork.Milestones.Update(item);
                }
                _unitOfWork.Save();

                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            // Read data from Excel
                            int milestoneId = int.Parse(worksheet.Cells[row, 1].Text);
                            string milestoneName = worksheet.Cells[row, 2].Text;
                            string milestoneDescription = worksheet.Cells[row, 3].Text;
                            double milestonePercentage = double.Parse(worksheet.Cells[row, 4].Text);
                            int subjectId = int.Parse(worksheet.Cells[row, 5].Text);
                            DateTime startDate = DateTime.Parse(worksheet.Cells[row, 6].Text);
                            DateTime endDate = DateTime.Parse(worksheet.Cells[row, 7].Text);

                            // Generate a unique milestone ID

                            if (!milestoneDictionary.ContainsKey(milestoneId))
                            {
                                var milestone = new MilestoneWithCriteriaModel
                                {
                                    MilestoneName = milestoneName,
                                    Description = milestoneDescription,
                                    Percentage = milestonePercentage,
                                    SubjectId = subjectId,
                                    StartDate = startDate,
                                    EndDate = endDate,
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now,
                                    IsDeleted = false,
                                    Criteria = new List<Criterion>()
                                };

                                milestoneDictionary[milestoneId] = milestone;
                            }

                            // Process Criteria
                            string criteriaName = worksheet.Cells[row, 8].Text;
                            string criteriaDescription = worksheet.Cells[row, 9].Text;
                            double criteriaPercentage = double.Parse(worksheet.Cells[row, 10].Text);

                            var criterion = new Criterion
                            {
                                CriteriaName = criteriaName,
                                Description = criteriaDescription,
                                Percentage = criteriaPercentage,
                                CreatedDate = DateTime.Now,
                                UpdatedDate = DateTime.Now,
                                IsDeleted = false
                            };

                            milestoneDictionary[milestoneId].Criteria.Add(criterion);
                        }
                    }
                }

                foreach (var milestone in milestoneDictionary.Values)
                {
                    // Check for duplicate milestones (optional, based on your requirements)
                    var existingMilestone = milestoneCurrent
                        .Where(m => m.MilestoneName == milestone.MilestoneName && m.SubjectId == milestone.SubjectId)
                        .FirstOrDefault();

                    if (existingMilestone != null)
                    {
                        throw new Exception($"Milestone '{milestone.MilestoneName}' already exists for Subject ID {milestone.SubjectId}.");
                    }

                    // Insert Milestone
                    var milestoneEntity = _mapper.Map<Milestone>(milestone);
                    await _unitOfWork.Milestones.InsertAsync(milestoneEntity);

                    // Insert Criteria
                    foreach (var criterion in milestoneEntity.Criteria)
                    {
                        criterion.MilestoneId = milestone.MilestoneId;
                        await _unitOfWork.Criterion.InsertAsync(criterion);
                    }
                    _unitOfWork.Save();

                }
                _unitOfWork.Save();
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Console.WriteLine(ex.Message);
                return false;
            }
        }


    }
}
