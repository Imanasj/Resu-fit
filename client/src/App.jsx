import { useEffect, useState } from "react";
import "./App.css";

function App() {
  const [resumeText, setResumeText] = useState("");
  const [jobText, setJobText] = useState("");
  const [jobTitle, setJobTitle] = useState("");

  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const [history, setHistory] = useState([]);
  const [selectedHistory, setSelectedHistory] = useState(null);

  // Load scan history when the page opens
  useEffect(() => {
    const savedHistory = localStorage.getItem("resufit-history");

    if (savedHistory) {
      try {
        setHistory(JSON.parse(savedHistory));
      } catch {
        setHistory([]);
      }
    }
  }, []);

  const saveHistory = (newItem) => {
    const updatedHistory = [newItem, ...history];

    setHistory(updatedHistory);

    localStorage.setItem(
      "resufit-history",
      JSON.stringify(updatedHistory)
    );
  };

  const handleAnalyze = async () => {
    if (!resumeText.trim() || !jobText.trim()) {
      setError("Please enter both resume text and job description.");
      return;
    }

    setLoading(true);
    setError("");
    setResult(null);
    setSelectedHistory(null);

    try {
      const response = await fetch("/api/scan", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },

        body: JSON.stringify({
          userId: "c7ec1007-f6c8-46b7-a304-8ade140df9fe",
          resumeText: resumeText,
          jobText: jobText,
          resumeFilename: "pasted-resume.txt",
          jobTitle: jobTitle || "Untitled Job",
        }),
      });

      const data = await response.json();

      if (!response.ok) {
        throw new Error(
          data.detail ||
            data.message ||
            "Something went wrong while analyzing the resume."
        );
      }

      setResult(data);

      const historyItem = {
        id: data.scanId || Date.now(),
        jobTitle: jobTitle || "Untitled Job",
        date: new Date().toLocaleString(),
        matchScore: data.matchScore,
        matchedKeywords: data.matchedKeywords || [],
        missingKeywords: data.missingKeywords || [],
      };

      saveHistory(historyItem);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleViewHistory = (item) => {
    setSelectedHistory(item);

    setTimeout(() => {
      document
        .getElementById("history-details")
        ?.scrollIntoView({ behavior: "smooth" });
    }, 100);
  };

  const handleClearHistory = () => {
    localStorage.removeItem("resufit-history");
    setHistory([]);
    setSelectedHistory(null);
  };

  return (
    <div className="app">
      <header className="header">
        <div>
          <h1>RESU-FIT</h1>
          <p>Resume & Job Match Scanner</p>
        </div>
      </header>

      <main className="container">
        <section className="intro">
          <h2>Analyze Your Resume</h2>

          <p>
            Compare your resume with a job posting and find out how well they
            match.
          </p>
        </section>

        <div className="form-card">
          <div className="form-group">
            <label htmlFor="jobTitle">Job Title</label>

            <input
              id="jobTitle"
              type="text"
              placeholder="Example: Software Developer"
              value={jobTitle}
              onChange={(e) => setJobTitle(e.target.value)}
            />
          </div>

          <div className="form-group">
            <label htmlFor="resumeText">Resume</label>

            <textarea
              id="resumeText"
              placeholder="Paste your resume text here..."
              value={resumeText}
              onChange={(e) => setResumeText(e.target.value)}
            />
          </div>

          <div className="form-group">
            <label htmlFor="jobText">Job Description</label>

            <textarea
              id="jobText"
              placeholder="Paste the job posting here..."
              value={jobText}
              onChange={(e) => setJobText(e.target.value)}
            />
          </div>

          {error && <div className="error-message">{error}</div>}

          <button
            className="analyze-button"
            onClick={handleAnalyze}
            disabled={loading}
          >
            {loading ? "Analyzing..." : "Analyze Resume"}
          </button>
        </div>

        {result && (
          <section className="results-card">
            <h2>Analysis Results</h2>

            <div className="score-section">
              <h3>Match Score</h3>

              <div className="score">
                {result.matchScore}%
              </div>
            </div>

            <div className="keyword-section">
              <div className="keyword-box">
                <h3>Matched Keywords</h3>

                {result.matchedKeywords &&
                result.matchedKeywords.length > 0 ? (
                  <ul>
                    {result.matchedKeywords.map((keyword, index) => (
                      <li key={index}>✓ {keyword}</li>
                    ))}
                  </ul>
                ) : (
                  <p>No matched keywords found.</p>
                )}
              </div>

              <div className="keyword-box">
                <h3>Missing Keywords</h3>

                {result.missingKeywords &&
                result.missingKeywords.length > 0 ? (
                  <ul>
                    {result.missingKeywords.map((keyword, index) => (
                      <li key={index}>✗ {keyword}</li>
                    ))}
                  </ul>
                ) : (
                  <p>No missing keywords.</p>
                )}
              </div>
            </div>
          </section>
        )}

        <section className="history-card">
          <div className="history-header">
            <h2>Scan History</h2>

            {history.length > 0 && (
              <button
                className="clear-button"
                onClick={handleClearHistory}
              >
                Clear History
              </button>
            )}
          </div>

          {history.length === 0 ? (
            <p className="empty-history">
              No scan history yet. Analyze a resume to create your first scan.
            </p>
          ) : (
            <div className="history-list">
              {history.map((item) => (
                <div className="history-item" key={item.id}>
                  <div className="history-info">
                    <h3>{item.jobTitle}</h3>
                    <p>{item.date}</p>
                  </div>

                  <div className="history-actions">
                    <span className="history-score">
                      {item.matchScore}%
                    </span>

                    <button
                      className="view-button"
                      onClick={() => handleViewHistory(item)}
                    >
                      View Details
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </section>

        {selectedHistory && (
          <section
            className="results-card"
            id="history-details"
          >
            <h2>History Details</h2>

            <h3 className="history-job-title">
              {selectedHistory.jobTitle}
            </h3>

            <p className="history-date">
              {selectedHistory.date}
            </p>

            <div className="score-section">
              <h3>Match Score</h3>

              <div className="score">
                {selectedHistory.matchScore}%
              </div>
            </div>

            <div className="keyword-section">
              <div className="keyword-box">
                <h3>Matched Keywords</h3>

                {selectedHistory.matchedKeywords.length > 0 ? (
                  <ul>
                    {selectedHistory.matchedKeywords.map(
                      (keyword, index) => (
                        <li key={index}>✓ {keyword}</li>
                      )
                    )}
                  </ul>
                ) : (
                  <p>No matched keywords found.</p>
                )}
              </div>

              <div className="keyword-box">
                <h3>Missing Keywords</h3>

                {selectedHistory.missingKeywords.length > 0 ? (
                  <ul>
                    {selectedHistory.missingKeywords.map(
                      (keyword, index) => (
                        <li key={index}>✗ {keyword}</li>
                      )
                    )}
                  </ul>
                ) : (
                  <p>No missing keywords.</p>
                )}
              </div>
            </div>
          </section>
        )}
      </main>
    </div>
  );
}

export default App;