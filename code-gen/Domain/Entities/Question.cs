using Core.SharedDomain.Audit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Question : AuditEntity
    {
        public virtual int Order { get; set; }
        public virtual string QuestionTitle { get; set; }
        public virtual string ArabicQuestionTitle { get; set; }
        public virtual string AnswerContent { get; set; }
        public virtual string ArabicAnswerContent { get; set; }
    }
}
