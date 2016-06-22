package dan.langford.firesneeze.ingest

class XmlToSql {

    static void main(String...run){
        new XmlToSql().run();
    }

    void run() {
        def xmlData = new XmlSlurper().parse(new File('./pacg/TextAsset/UI.txt'))
        print(xmlData.Entries)
    }
}
