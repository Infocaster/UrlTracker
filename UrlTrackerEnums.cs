namespace InfoCaster.Umbraco.UrlTracker
{
	public enum UrlTrackerEntryType
	{
		Redirect,
		NotFound
	}

	public enum UrlTrackerRedirectType
	{
		MovedPermanently = 301,
		TemporaryRedirect = 302
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