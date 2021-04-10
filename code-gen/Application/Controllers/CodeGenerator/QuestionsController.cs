using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationShared.Entites.Question;
using Core.SharedDomain.Security;
using Domain.Entities;
using EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Controllers.CodeGenerator
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
    private readonly UserManager<User> _userManager;
    private readonly CodeContext _context;

    public QuestionsController(UserManager<User> userManager, CodeContext context)
    {
        _userManager = userManager;
        _context = context;
    }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionOutputDto>>> GetQuestions()
        {
            var data = await _context.Questions.Where(x => !x.VirtualDeleted).OrderBy(x=> x.Order).ToListAsync();
            var all = data.Select(x => new QuestionOutputDto()
            {
                id = x.Id,
                active = x.Order == 1 ? true : false,
                Order = x.Order,
                AnswerContent = x.AnswerContent,
                ArabicAnswerContent = x.ArabicAnswerContent,
                ArabicQuestionTitle = x.ArabicQuestionTitle,
                QuestionTitle = x.QuestionTitle
            });
            
            var allDataList = new List<QuestionOutputDto>((all as IEnumerable<QuestionOutputDto>).AsQueryable());
            return allDataList;
        }

        // GET: api/Brands/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            return question;
        }

        // PUT: api/Brands/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Question>> PutBrand(int id, QuestionInputDto input)
        {
            var question = await _context.Questions.FindAsync(id);
            question.QuestionTitle = input.QuestionTitle;
            question.ArabicQuestionTitle = input.ArabicQuestionTitle;
            question.AnswerContent = input.AnswerContent;
            question.ArabicAnswerContent = input.ArabicAnswerContent;
            question.Order =(int) input.Order.Value;
            question.UpdatedUserId = input.UserId;
            question.UpdatedDate = DateTime.Now;
            _context.Entry(question).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return question;
        }

        // POST: api/Brands
        [HttpPost]
        public async Task<ActionResult<Question>> PostBrand(QuestionInputDto input)
        {
            try
            {
                var question = new Question()
                {
                    QuestionTitle = input.QuestionTitle,
                    ArabicQuestionTitle = input.ArabicQuestionTitle,
                    AnswerContent = input.AnswerContent,
                    ArabicAnswerContent = input.ArabicAnswerContent,
                    Order = (int)input.Order.Value,
                CreatedDate = DateTime.Now,
                    CreatedUserId = input.UserId
                };
                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                return question;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // DELETE: api/Brands/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Question>> DeleteBrand(int id, string userId)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            question.DeletedDate = DateTime.Now;
            question.DeletedUserId = userId;
            question.VirtualDeleted = true;
            _context.Entry(question).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return question;
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
    }
}
