const ScoreCircle = ({ score = 0 }) => {
  const numericScore = Math.max(0, Math.min(100, Number(score) || 0));
  const radius = 52;
  const circumference = 2 * Math.PI * radius;
  const offset = circumference - (numericScore / 100) * circumference;

  return (
    <div className="score-circle" aria-label={`Match score ${numericScore}%`}>
      <svg width="170" height="170" viewBox="0 0 170 170" role="img">
        <circle className="score-circle-bg" cx="85" cy="85" r={radius} />
        <circle
          className="score-circle-progress"
          cx="85"
          cy="85"
          r={radius}
          strokeDasharray={circumference}
          strokeDashoffset={offset}
        />
      </svg>
      <div className="score-circle-label">{Math.round(numericScore)}%</div>
    </div>
  );
};

export default ScoreCircle;
