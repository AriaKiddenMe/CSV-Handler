using System.IO;
using System.Reflection.PortableExecutable;

namespace CSV_Handler;

/**
 * Encapsulates the contents of a comma seperated value(.csv) file type, returning values as either:
 *      one row of comma seperated values (array of strings)
 *      the remaining rows of comma seperated values (2D array of strings
 */
public class CSVReader
{
    private static string currentWorkingDirectory = Directory.GetCurrentDirectory();
    private String CSVFileString;
    
    /**
     * Takes the concatenated string of the relative path followed by the file name.
     *      The path and file name are both required.
     *      Any file name extensions at the end of the file name must be ".csv" (case sensitive)
     */
    public CSVReader(String relativePathNFileName)
    {
        //parameter testing
        if(String.IsNullOrEmpty(relativePathNFileName)) throw new ArgumentException("An empty string was passed into the CSVReader");

        string csvFileName;
        string csvAbsPath;
        
        //further input testing
        if (!relativePathNFileName.Contains('\\')) {
            if (relativePathNFileName.Contains('/'))
                throw new ArgumentException("File path passed to CSVReader was encoded with \'/\' rather than \'\\\'");
            throw new ArgumentException("no file path was passed into the CSVReader");
        }

        var csvRelativePath = relativePathNFileName.Substring(0, relativePathNFileName.LastIndexOf('\\'));
        csvFileName = relativePathNFileName.Substring(relativePathNFileName.LastIndexOf('\\') + 1);
            
        checkFilePathAndName(ref csvRelativePath, ref csvFileName);

        csvAbsPath = FindAbsPath(csvRelativePath);
        if (csvAbsPath == null) throw new IOException(String.Format("Cannot find directory of the CSV file, Directory: {0}.", relativePathNFileName.Substring(0, relativePathNFileName.LastIndexOf('\\'))));

        CSVFileString = getFileContents(String.Concat(csvAbsPath, csvFileName));
    }

    /**
     * Takes two parameter strings representing the relative path and the file name.
     *      The path and file name are both required.
     *      Any file name extensions at the end of the file name must be ".csv" (case sensitive)
     */
    public CSVReader(string relativePath, string csvFileName)
    {
        //general parameter testing
        checkFilePathAndName(ref relativePath,ref csvFileName);
        var csvAbsPath = FindAbsPath(relativePath.Substring(0, relativePath.LastIndexOf('\\')));
        if (csvAbsPath == null) throw new IOException(String.Format("Cannot find directory of the CSV file, Directory: {0}.", relativePath));
        
        CSVFileString = getFileContents(String.Concat(csvAbsPath, csvFileName));
    }

    private void checkFilePathAndName(ref string relativePath, ref string fileName)
    {
        if(String.IsNullOrEmpty(relativePath) || String.IsNullOrEmpty(fileName)) throw new ArgumentException("An empty name or path was passed into the CSVReader");
        
        //path parameter semi-validation testing
        if (!relativePath.Contains('\\')) {
            if (relativePath.Contains('/')) throw new ArgumentException("File path passed to CSVReader was encoded with \'/\' rather than \'\\\\\'");
            throw new ArgumentException("no file path was passed into the CSVReader");
        }

        if (fileName.Contains('.'))
        {
            if (!((fileName.Substring(fileName.LastIndexOf('.'), fileName.Length - 1)).Equals(".csv")))
                throw new ArgumentException("File type passed in to CSVReader is not a Comma Seperated Values File (\".csv\")");
            if (fileName.Substring(0, fileName.LastIndexOf(".")).Length < 1)
                throw new ArgumentException("File name not given to CSVReader");
        } else
        {
            fileName = (fileName + ".csv");
        }
        if(!relativePath.EndsWith('\\')) relativePath = relativePath + '\\';
    }
    
    //NOTE input is trusted so absolutePath should be valid and FileName should be >= 1 char
    private String getFileContents(string absolutePathNFileName)
    {
        return File.ReadAllText(absolutePathNFileName);

    }

    private string FindAbsPath(String givenPath)
    {
        var current = currentWorkingDirectory;
        var dir = new DirectoryInfo(current);

        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, givenPath);
            if (Directory.Exists(candidate))
                return candidate;

            dir = dir.Parent;
        }

        return null;
    }
}