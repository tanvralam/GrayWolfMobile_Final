using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
    public interface ISendEmail
    {
        Task ComposeEmail(
            string emailAddress,
            string subject, 
            string messageBody, 
            string filePath,
            IEnumerable<string> attachments);
    }
}
