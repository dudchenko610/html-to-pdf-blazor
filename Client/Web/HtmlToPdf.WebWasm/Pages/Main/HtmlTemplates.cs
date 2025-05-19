namespace HtmlToPdf.WebWasm.Pages.Main;

public static class HtmlTemplates
{
    public const string Template1 = @"
      <html>
      <head>
        <meta charset=""utf-8"" />
        <style>
          @page {
            size: A4;
            margin: 20mm;
          }

          * {
            box-sizing: border-box;
          }

          html, body {
            margin: 0;
            padding: 0;
            width: 210mm;
            height: 297mm;
            font-family: Arial, sans-serif;
            color: #000;
            background: #fff;
          }

          body {
            padding: 20mm;
            display: flex;
            flex-direction: column;
            gap: 16px;
          }

          h1, h2, h3 {
            margin: 0 0 6px 0;
          }

          p {
            margin: 0 0 6px 0;
          }

          .section {
            margin-top: 12px;
          }

          .header {
            border-bottom: 2px solid #000;
            padding-bottom: 10px;
          }

          .header h1 {
            font-size: 24pt;
          }

          .contact {
            font-size: 10pt;
          }

          .profile, .experience, .education, .skills {
            font-size: 11pt;
          }

          .job, .education-entry {
            margin-bottom: 10px;
          }

          .job h3, .education-entry h3 {
            font-size: 12pt;
          }

          .job p, .education-entry p {
            font-size: 10pt;
          }

          .skills ul {
            list-style: none;
            padding: 0;
            margin: 0;
            columns: 2;
          }

          .skills li {
            margin-bottom: 4px;
          }
        </style>
      </head>
      <body>
        <div class=""header"">
          <h1>John Doe</h1>
          <div class=""contact"">
            <p>Email: john.doe@example.com | Phone: +123 456 7890</p>
            <p>LinkedIn: linkedin.com/in/johndoe | Location: City, Country</p>
          </div>
        </div>

        <div class=""section profile"">
          <h2>Profile</h2>
          <p>A motivated professional with X years of experience in [industry]. Passionate about [something], with proven expertise in [skill areas].</p>
        </div>

        <div class=""section experience"">
          <h2>Experience</h2>

          <div class=""job"">
            <h3>Senior Developer – Company A</h3>
            <p>Jan 2020 – Present</p>
            <p>Key achievements: Led a team of developers, improved system performance by 30%...</p>
          </div>

          <div class=""job"">
            <h3>Developer – Company B</h3>
            <p>Aug 2016 – Dec 2019</p>
            <p>Worked on full-stack development, contributed to large-scale web apps...</p>
          </div>
        </div>

        <div class=""section education"">
          <h2>Education</h2>

          <div class=""education-entry"">
            <h3>B.Sc. in Computer Science – University X</h3>
            <p>2012 – 2016</p>
          </div>
        </div>

        <div class=""section skills"">
          <h2>Skills</h2>
          <ul>
            <li>JavaScript</li>
            <li>Python</li>
            <li>React</li>
            <li>Node.js</li>
            <li>SQL</li>
            <li>Docker</li>
            <li>Git</li>
            <li>REST APIs</li>
          </ul>
        </div>
      </body>
      </html>
";
    public const string Template2 = @"
        <html>
        <head>
          <meta charset=""utf-8"" />
          <style>
            @page {
              size: A4;
              margin: 20mm;
            }

            * {
              box-sizing: border-box;
            }

            html, body {
              margin: 0;
              padding: 0;
              width: 210mm;
              height: 297mm;
              font-family: Arial, sans-serif;
              background: #fff;
              color: #000;
            }

            body {
              padding: 20mm;
              display: flex;
              flex-direction: column;
              gap: 20px;
            }

            h1, h2 {
              margin-bottom: 10px;
            }

            .section {
              margin-bottom: 20px;
            }

            label {
              display: block;
              font-weight: bold;
              margin-top: 10px;
              margin-bottom: 4px;
            }

            input, textarea, select {
              width: 100%;
              padding: 6px;
              font-size: 11pt;
              border: 1px solid #ccc;
              border-radius: 3px;
            }

            .row {
              display: flex;
              gap: 16px;
            }

            .col {
              flex: 1;
            }

            .signature {
              margin-top: 40px;
            }

            .signature-line {
              margin-top: 30px;
              border-top: 1px solid #000;
              width: 60%;
            }

            .date-field {
              width: 40%;
              margin-top: 10px;
            }
          </style>
        </head>
        <body>

          <h1>Application Form</h1>

          <div class=""section"">
            <h2>Personal Information</h2>
            <div class=""row"">
              <div class=""col"">
                <label for=""firstName"">First Name</label>
                <input type=""text"" id=""firstName"" />
              </div>
              <div class=""col"">
                <label for=""lastName"">Last Name</label>
                <input type=""text"" id=""lastName"" />
              </div>
            </div>

            <label for=""email"">Email Address</label>
            <input type=""email"" id=""email"" />

            <label for=""phone"">Phone Number</label>
            <input type=""text"" id=""phone"" />

            <label for=""address"">Address</label>
            <textarea id=""address"" rows=""2""></textarea>
          </div>

          <div class=""section"">
            <h2>Position Applied For</h2>
            <label for=""position"">Position</label>
            <input type=""text"" id=""position"" />

            <label for=""startDate"">Available Start Date</label>
            <input type=""date"" id=""startDate"" />
          </div>

          <div class=""section"">
            <h2>Previous Employment</h2>
            <label for=""employer"">Most Recent Employer</label>
            <input type=""text"" id=""employer"" />

            <label for=""jobTitle"">Job Title</label>
            <input type=""text"" id=""jobTitle"" />

            <label for=""duration"">Duration</label>
            <input type=""text"" id=""duration"" placeholder=""e.g. Jan 2020 – Dec 2023"" />

            <label for=""duties"">Key Duties</label>
            <textarea id=""duties"" rows=""4""></textarea>
          </div>
        </body>
        </html>
        ";
}