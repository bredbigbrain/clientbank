using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Bot_Assistant
    {
        public string Answer(string question)
        {
            Regex regex = new Regex("Вернуть", RegexOptions.IgnoreCase);   
            if (regex.IsMatch(question))
            {
                regex = new Regex("кредит", RegexOptions.IgnoreCase);
                if (regex.IsMatch(question))
                {
                    return "Кому это вы дали кредит? \nНо если вы хотите оплатить свой кредит: выберите клиената из списка справа,\n выберите кредит в списке операций, нажмите на кнопку платеж по кредиту.";
                }
                else
                {
                    regex = new Regex("деньги", RegexOptions.IgnoreCase);
                    if (regex.IsMatch(question))
                    {
                        return "Если вы хотите вернуть посланые кому-то деньги: выберите клиената из списка справа,\n выберите тразакцию, нажмите кнопку отмена транзакции";
                    }
                }
                return "Что вернуть?";
            }

            regex = new Regex("перевести", RegexOptions.IgnoreCase);
            if (regex.IsMatch(question))
            {
                regex = new Regex("деньги", RegexOptions.IgnoreCase);
                if (regex.IsMatch(question))
                {
                    return "Если вы хотите превести кому-нибудь деньги: нажмите кнопку перевод средств,\n выберите отправителя, выберите получаетля, введите сумму, жмякните кнопку";
                }
                return "Кого перевести?";
            }


            regex = new Regex("кредит", RegexOptions.IgnoreCase);
            if (regex.IsMatch(question))
            {
                regex = new Regex("получить", RegexOptions.IgnoreCase);
                if (regex.IsMatch(question))
                {
                    return "Если вы хотите плучить кредит: выберите клиента, введите желаемую сумму и срок\n жмякните кнопку";
                }
                regex = new Regex("взять", RegexOptions.IgnoreCase);
                if (regex.IsMatch(question))
                {
                    return "Если вы хотите взять кредит: выберите клиента, введите желаемую сумму и срок\n жмякните кнопку";
                }

                return "Что что кредит?";
            }

            regex = new Regex("как", RegexOptions.IgnoreCase);
            if (regex.IsMatch(question))
            {
                regex = new Regex("дела", RegexOptions.IgnoreCase);
                if (regex.IsMatch(question))
                {
                    return "Уж получше чем у тебя, кожанный мешок!";
                }
            }

            return "Я чот вас не понял";
        }
    }
}
