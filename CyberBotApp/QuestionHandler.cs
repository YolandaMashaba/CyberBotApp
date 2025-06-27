using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberBotApp
{
    public class QuestionHandler
    {
        private List<string> replies = new List<string>();
        private List<string> ignore = new List<string>();
        private MemoryManager memoryManager = new MemoryManager();

        private Dictionary<string, string> sentimentResponses = new Dictionary<string, string>()
        {
            {"worried", "I see you're worried. Let's talk about it."},
            {"concerned", "I understand your concern. Cybersecurity is important."},
            {"help", "I can help you with that. Cybersecurity is crucial."},
            {"feelings", "I understand your feelings. Cybersecurity is a big deal."},
            {"scared", "I understand that you're scared. Let's work through it together."},
            {"nervous", "It's normal to feel nervous. I'm here to help."},
            {"anxious", "I see you're anxious. Let's address your concerns."},
            {"overwhelmed", "I understand that you're overwhelmed. Let's break it down."},
            {"stressed", "I can tell that you're stressed. Let me help ease that step by step."},
            {"unsafe", "I understand that you feel unsafe. Let's discuss how to improve your security."}
        };

        public QuestionHandler(MemoryManager memoryManager)
        {
            this.memoryManager = memoryManager;
            StoreIgnore();
            StoreReplies();
        }

        // Main method you will call from your WPF UI
        public List<string> GetResponse(string userInput)
        {
            List<string> responses = new List<string>();
            List<string> historyEntries = new List<string>();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                responses.Add("Chat AI -> Please enter a valid question.");
                return responses;
            }

            string question = userInput.ToLower();
            string[] words = question.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);


            List<string> filteredWords = words.Where(w => !ignore.Contains(w)).ToList();

            HashSet<string> matchedReplies = new HashSet<string>();
            foreach (string reply in replies)
            {
                foreach (string word in filteredWords)
                {
                    if (reply.StartsWith(word + ":", StringComparison.OrdinalIgnoreCase))
                        matchedReplies.Add(reply);
                }
            }

            foreach (string word in filteredWords)
            {
                if (sentimentResponses.ContainsKey(word))
                {
                    responses.Add("Chat AI -> " + sentimentResponses[word]);
                    historyEntries.Add("User -> " + userInput);
                    historyEntries.Add("Chat AI -> " + sentimentResponses[word]);
                }
            }

            if (matchedReplies.Count > 0)
            {
                var grouped = matchedReplies.GroupBy(r => r.Split(':')[0]);
                Random rand = new Random();
                foreach (var group in grouped)
                {
                    string reply = group.OrderBy(x => rand.Next()).First();
                    responses.Add("Chat AI -> " + reply);
                    historyEntries.Add("User -> " + userInput);
                    historyEntries.Add("Chat AI -> " + reply);
                }
            }
            else if (!sentimentResponses.Keys.Any(word => filteredWords.Contains(word)))
            {
                string noMatch = "I'm only allowed to provide information about cybersecurity. Please ask a cybersecurity-related question.";
                responses.Add("Chat AI -> " + noMatch);
                historyEntries.Add("User -> " + userInput);
                historyEntries.Add("Chat AI -> " + noMatch);
            }

            memoryManager.SaveConversation(historyEntries);
            return responses;
        }

        private void StoreReplies()
        {
            replies = new List<string>
            {
                "phishing: Be cautious of emails or messages asking for personal details; verify the sender.",
                "phishing: Don’t click on suspicious links or attachments in unsolicited emails.",
                "phishing: Hover over links to verify their destination before clicking.",
                "phishing: Check for grammatical errors and unusual requests in emails.",
                "phishing: Always verify the sender’s email address, even if it looks familiar.",

                "cybersecurity: Cybersecurity is the practice of protecting systems and data from digital attacks.",
                "cybersecurity: Cybersecurity involves defending computers, servers, and data from malicious attacks.",
                "cybersecurity: Stay updated with software patches to protect against known vulnerabilities.",
                "cybersecurity: Being aware and informed is the first step in strong cybersecurity practices.",
                "cybersecurity: Regularly audit your digital security to stay protected from evolving threats.",

                "password: Always use strong passwords with a mix of letters, numbers, and symbols.",
                "password: Use complex passwords with at least 12 characters, including symbols and numbers.",
                "password: Avoid using the same password for multiple accounts.",
                "password: Change your passwords regularly and avoid dictionary words.",
                "password: Consider using a password manager to generate and store secure passwords.",

                "malware: Avoid downloading files from unknown sources to prevent malware infections.",
                "firewall: Firewalls help block unauthorized access to your network.",
                "vpn: A VPN encrypts your internet traffic, keeping your online activity private.",
                "encryption: Encryption helps protect your sensitive data from unauthorized access.",
                "2fa: Two-Factor Authentication adds an extra layer of security to your accounts.",
                "ransomware: Never open suspicious attachments, and always back up your important files.",
                "antivirus: Keep your antivirus software updated to protect against threats.",
                "social engineering: Cybercriminals use manipulation to steal confidential information, stay alert.",
                "hacking: Ethical hackers help organizations secure their systems, but illegal hacking is a crime.",
                "data breach: A data breach is a security incident where sensitive information is exposed."
            };
        }

        private void StoreIgnore()
        {
            ignore = new List<string>
            {
                "is", "the", "a", "an", "what", "how", "there", "why", "when", "where", "does", "do", "tell",
                "are", "can", "could", "should", "would", "i", "me", "my", "you", "your", "we", "our",
                "it", "to", "this", "that", "these", "those", "has", "have", "had", "was", "were",
                "and", "or", "but", "so", "on", "at", "with", "by", "from", "in", "of", "for", "about"
            };
        }
    }
}