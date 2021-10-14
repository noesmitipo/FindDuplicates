EXPLANATION

At the beginning my idea was to compare file names all around the drive to look for duplicates.
When I saw the example result you provide in the assignment, I realised the attacker could have not only duplicated the files, but also changed the names.
This changed the whole problem, since now we need to compare the file content and that's much slower.

First I get all the file information from files in a specific root and its subfolders (it could be main root C:\).
Then I discard the unique files we can find here. Since there are a lot of files, if we can avoid to read data from some of them for the comparison, we win some time.
If we know that all the files have been duplicated we can remove this part, because it will do nothing, and save some time.

Then we compare the files we have remaining with the ones with the same length.
We assume all the files with different lengths are different, so we don't have to waste time comparing with the rest.

Since our files can only be duplicated once, if a file has already be compared, we add it to a list to not compare it again.
Same for the files who match the one we are comparing with.

The cost of my solution is cuadratic since we are using two foreach to go through the elements, even if we are not comparing every element and just the ones with the same length.

If we want a cost O(n) but we have to compare the bytes/content of the files, I don't know if the memory could support it.
Maybe if we load all the content we could just use a single foreach and do something like this.

foreach (File file in filesToCompare)
{
	if(filesToCompare.Any(f => f.Content == file.Content)) {
		//Get originalPath and duplicatedPath here
	    duplicatedFiles.Add(new DuplicatedFile(originalPath, duplicatedPath));
	}
}

We could also compare FileInfos by name and length, and if some of them still have the same name, it'd be much faster, even if we lose some accuracy.