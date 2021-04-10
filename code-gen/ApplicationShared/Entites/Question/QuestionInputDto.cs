using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Entites.Question
{
    public class QuestionInputDto
    {
        public int? id { get; set; }
        public int? Order { get; set; }
        public string QuestionTitle { get; set; }
        public string ArabicQuestionTitle { get; set; }
        public string AnswerContent { get; set; }
        public string ArabicAnswerContent { get; set; }
        public string UserId { get; set; }
    }
}
