export function PrivacyPage() {
  return (
    <div className="mx-auto max-w-3xl px-4 py-10">
      <h1 className="text-3xl font-semibold tracking-tight text-ink">Privacy</h1>
      <div className="mt-6 grid gap-4 text-slate-700">
        <p>No account is required. No password is collected.</p>
        <p>CV uploads are processed temporarily in memory while a request is handled.</p>
        <p>CVs should not be permanently stored. JobScout V1 does not write uploaded files to disk or a database.</p>
        <p>Job information comes from external sources or, in V1, clearly labelled demo data.</p>
        <p>Job availability may change. Users should verify information on the original listing.</p>
        <p>JobScout does not guarantee employment. Match scores are estimates.</p>
        <p>No unnecessary personal information should be collected.</p>
      </div>
    </div>
  )
}
