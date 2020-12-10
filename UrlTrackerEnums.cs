namespace InfoCaster.Umbraco.UrlTracker
{
	public enum UrlTrackerEntryType
	{
		Redirect,
		NotFound
	}

	public enum UrlTrackerHttpCode
	{
		MovedPermanently = 301,
		TemporaryRedirect = 302,
		Gone = 410
	}

	public enum UrlTrackerSortType
	{
		CreatedDesc,
		CreatedAsc,
		OccurrencesDesc,
		OccurrencesAsc,
		LastOccurrenceDesc,
		LastOccurrenceAsc
	}

	public enum UrlTrackerReason
	{
		Renamed,
		Moved,
		UrlOverwritten,
		UrlOverwrittenSEOMetadata
	}
}