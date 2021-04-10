using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Entites.Question
{
    public class QuestionOutputDto
    {
        public int? id { get; set; }
        public int? Order { get; set; }
        public bool active { get; set; }
        public string QuestionTitle { get; set; }
        public string ArabicQuestionTitle { get; set; }
        public string AnswerContent { get; set; }
        public string ArabicAnswerContent { get; set; }
    }
}
