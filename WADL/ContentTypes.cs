using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WADL
{
    //http://en.wikipedia.org/wiki/MIME
    //http://www.w3schools.com/media/media_mimeref.asp
    public class ContentTypes
    {
        public const string unknown = "application/x-unknown";
        public const string vcard = "text/vcard";
        public const string vcf = "text/vcard";
        public const string vcs = "text/x-vCalendar";
        public const string atom = "application/atom+xml";
        public const string json = "application/json";
        public const string evy = "application/envoy";
        public const string octetStream = "application/octet-stream";
        public const string wadl = "application/vnd.sun.wadl+xml";
        public const string fif = "application/fractals"; 
        public const string spl = "application/futuresplash";  
        public const string hta = "application/hta";  
        public const string acx = "application/internet-property-stream";  
        public const string hqx = "application/mac-binhex40"; 
        public const string doc = "application/msword";  
        public const string dot = "application/msword";  
        public const string bin = "application/octet-stream";  
        public const string @class = "application/octet-stream";  
        public const string dms = "application/octet-stream";  
        public const string exe  = "application/octet-stream";
        public const string lha = "application/octet-stream"; 
        public const string lzh = "application/octet-stream"; 
        public const string oda = "application/oda"; 
        public const string axs = "application/olescript"; 
        public const string pdf = "application/pdf"; 
        public const string prf = "application/pics-rules"; 
        public const string p10 = "application/pkcs10"; 
        public const string crl = "application/pkix-crl"; 
        public const string ai = "application/postscript"; 
        public const string eps = "application/postscript"; 
        public const string ps = "application/postscript";  
        public const string rtf = "application/rtf"; 
        public const string setpay = "application/set-payment-initiation"; 
        public const string setreg = "application/set-registration-initiation"; 
        public const string xla = "application/vnd.ms-excel"; 
        public const string xlc = "application/vnd.ms-excel"; 
        public const string xlm = "application/vnd.ms-excel"; 
        public const string xls = "application/vnd.ms-excel"; 
        public const string xlt = "application/vnd.ms-excel"; 
        public const string xlw = "application/vnd.ms-excel"; 
        public const string msg = "application/vnd.ms-outlook"; 
        public const string sst = "application/vnd.ms-pkicertstore"; 
        public const string cat = "application/vnd.ms-pkiseccat"; 
        public const string stl = "application/vnd.ms-pkistl"; 
        public const string pot = "application/vnd.ms-powerpoint"; 
        public const string pps = "application/vnd.ms-powerpoint"; 
        public const string ppt = "application/vnd.ms-powerpoint"; 
        public const string mpp = "application/vnd.ms-project"; 
        public const string wcm = "application/vnd.ms-works"; 
        public const string wdb = "application/vnd.ms-works"; 
        public const string wks = "application/vnd.ms-works"; 
        public const string wps = "application/vnd.ms-works"; 
        public const string hlp = "application/winhlp"; 
        public const string bcpio = "application/x-bcpio"; 
        public const string z = "application/x-compress"; 
        public const string tgz = "application/x-compressed"; 
        public const string cpio = "application/x-cpio"; 
        public const string csh = "application/x-csh"; 
        public const string dcr = "application/x-director"; 
        public const string dir = "application/x-director"; 
        public const string dxr = "application/x-director"; 
        public const string dvi = "application/x-dvi"; 
        public const string gtar = "application/x-gtar"; 
        public const string gz = "application/x-gzip";
        public const string hdf = "application/x-hdf"; 
        public const string ins = "application/x-internet-signup"; 
        public const string isp = "application/x-internet-signup"; 
        public const string iii = "application/x-iphone"; 
        public const string js = "application/x-javascript"; 
        public const string latex = "application/x-latex"; 
        public const string mdb = "application/x-msaccess"; 
        public const string crd = "application/x-mscardfile"; 
        public const string clp = "application/x-msclip"; 
        public const string dll = "application/x-msdownload"; 
        public const string m13 = "application/x-msmediaview"; 
        public const string m14 = "application/x-msmediaview"; 
        public const string mvb = "application/x-msmediaview"; 
        public const string wmf = "application/x-msmetafile"; 
        public const string mny = "application/x-msmoney"; 
        public const string pub = "application/x-mspublisher"; 
        public const string scd = "application/x-msschedule"; 
        public const string trm = "application/x-msterminal"; 
        public const string wri = "application/x-mswrite"; 
        public const string cdf = "application/x-netcdf"; 
        public const string nc = "application/x-netcdf"; 
        public const string pma = "application/x-perfmon"; 
        public const string pmc = "application/x-perfmon"; 
        public const string pml = "application/x-perfmon"; 
        public const string pmr = "application/x-perfmon"; 
        public const string pmw = "application/x-perfmon"; 
        public const string p12 = "application/x-pkcs12"; 
        public const string pfx = "application/x-pkcs12"; 
        public const string p7b = "application/x-pkcs7-certificates"; 
        public const string spc = "application/x-pkcs7-certificates"; 
        public const string p7r = "application/x-pkcs7-certreqresp"; 
        public const string p7c = "application/x-pkcs7-mime"; 
        public const string p7m = "application/x-pkcs7-mime"; 
        public const string p7s = "application/x-pkcs7-signature"; 
        public const string sh = "application/x-sh"; 
        public const string shar = "application/x-shar"; 
        public const string swf = "application/x-shockwave-flash"; 
        public const string sit = "application/x-stuffit"; 
        public const string sv4cpio = "application/x-sv4cpio"; 
        public const string sv4crc = "application/x-sv4crc"; 
        public const string tar = "application/x-tar"; 
        public const string tcl = "application/x-tcl"; 
        public const string tex = "application/x-tex"; 
        public const string texi = "application/x-texinfo"; 
        public const string texinfo = "application/x-texinfo"; 
        public const string roff = "application/x-troff"; 
        public const string t = "application/x-troff"; 
        public const string tr = "application/x-troff"; 
        public const string man = "application/x-troff-man"; 
        public const string me = "application/x-troff-me"; 
        public const string ms = "application/x-troff-ms"; 
        public const string ustar = "application/x-ustar"; 
        public const string src = " application/x-wais-source"; 
        public const string cer = " application/x-x509-ca-cert"; 
        public const string crt = "application/x-x509-ca-cert"; 
        public const string der = "application/x-x509-ca-cert"; 
        public const string pko = "application/ynd.ms-pkipko"; 
        public const string zip = "application/zip"; 
        public const string au = "audio/basic"; 
        public const string snd = "audio/basic"; 
        public const string mid = "audio/mid"; 
        public const string rmi = "audio/mid"; 
        public const string mp3 = "audio/mpeg"; 
        public const string aif = "audio/x-aiff";
        public const string aifc = "audio/x-aiff"; 
        public const string aiff = "audio/x-aiff"; 
        public const string m3u = "audio/x-mpegurl"; 
        public const string ra = "audio/x-pn-realaudio"; 
        public const string ram = "audio/x-pn-realaudio"; 
        public const string wav = "audio/x-wav"; 
        public const string bmp = "image/bmp"; 
        public const string cod = "image/cis-cod"; 
        public const string gif = "image/gif"; 
        public const string ief = "image/ief"; 
        public const string jpe = "image/jpeg"; 
        public const string jpeg = "image/jpeg"; 
        public const string jpg = "image/jpeg"; 
        public const string jfif = "image/pipeg"; 
        public const string svg = "image/svg+xml"; 
        public const string tif = "image/tiff"; 
        public const string tiff = "image/tiff"; 
        public const string ras = "image/x-cmu-raster"; 
        public const string cmx = "image/x-cmx"; 
        public const string ico = "image/x-icon";
        public const string pnm = "image/x-portable-anymap"; 
        public const string pbm = "image/x-portable-bitmap"; 
        public const string pgm = "image/x-portable-graymap"; 
        public const string ppm = "image/x-portable-pixmap"; 
        public const string rgb = "image/x-rgb"; 
        public const string xbm = "image/x-xbitmap";
        public const string xpm = "image/x-xpixmap"; 
        public const string xwd = "image/x-xwindowdump"; 
        public const string mht = "message/rfc822"; 
        public const string mhtml = "message/rfc822"; 
        public const string nws = "message/rfc822"; 
        public const string css = "text/css"; 
        public const string h323 = "text/h323"; 
        public const string htm = "text/html"; 
        public const string html = "text/html"; 
        public const string xht = "application/xhtml+xml";
        public const string xhtml = "application/xhtml+xml";
        public const string xml = "application/xml";
        public const string rss = "application/rss+xml";
        public const string xsl = "application/xml";
        public const string xslt = "application/xslt+xml";
        public const string stm = "text/html"; 
        public const string uls = "text/iuls"; 
        public const string bas = "text/plain"; 
        public const string c = "text/plain"; 
        public const string h = "text/plain"; 
        public const string txt = "text/plain"; 
        public const string rtx = "text/richtext"; 
        public const string sct = "text/scriptlet"; 
        public const string tsv = "text/tab-separated-values"; 
        public const string htt = "text/webviewhtml"; 
        public const string htc = "text/x-component"; 
        public const string etx = "text/x-setext"; 
        public const string mp2 = "video/mpeg"; 
        public const string mpa = "video/mpeg"; 
        public const string mpe = "video/mpeg"; 
        public const string mpeg = "video/mpeg"; 
        public const string mpg = "video/mpeg"; 
        public const string mpv2 = "video/mpeg"; 
        public const string mov = "video/quicktime"; 
        public const string qt = "video/quicktime"; 
        public const string lsf = "video/x-la-asf"; 
        public const string lsx = "video/x-la-asf"; 
        public const string asf = "video/x-ms-asf"; 
        public const string asr = "video/x-ms-asf"; 
        public const string asx = "video/x-ms-asf"; 
        public const string avi = "video/x-msvideo"; 
        public const string movie = "video/x-sgi-movie"; 
        public const string flr = "x-world/x-vrml"; 
        public const string vrml = "x-world/x-vrml"; 
        public const string wrl = "x-world/x-vrml"; 
        public const string wrz = "x-world/x-vrml"; 
        public const string xaf = "x-world/x-vrml"; 

    }
}

/*
 ai 	application/postscript
aif 	audio/x-aiff
aifc 	audio/x-aiff
aiff 	audio/x-aiff
asc 	text/plain
atom 	application/atom+xml
au 	audio/basic
avi 	video/x-msvideo
bcpio 	application/x-bcpio
bin 	application/octet-stream
bmp 	image/bmp
cdf 	application/x-netcdf
cgm 	image/cgm
class 	application/octet-stream
cpio 	application/x-cpio
cpt 	application/mac-compactpro
csh 	application/x-csh
css 	text/css
dcr 	application/x-director
dif 	video/x-dv
dir 	application/x-director
djv 	image/vnd.djvu
djvu 	image/vnd.djvu
dll 	application/octet-stream
dmg 	application/octet-stream
dms 	application/octet-stream
doc 	application/msword
dtd 	application/xml-dtd
dv 	video/x-dv
dvi 	application/x-dvi
dxr 	application/x-director
eps 	application/postscript
etx 	text/x-setext
exe 	application/octet-stream
ez 	application/andrew-inset
gif 	image/gif
gram 	application/srgs
grxml 	application/srgs+xml
gtar 	application/x-gtar
hdf 	application/x-hdf
hqx 	application/mac-binhex40
htm 	text/html
html 	text/html
ice 	x-conference/x-cooltalk
ico 	image/x-icon
ics 	text/calendar
ief 	image/ief
ifb 	text/calendar
iges 	model/iges
igs 	model/iges
jnlp 	application/x-java-jnlp-file
jp2 	image/jp2
jpe 	image/jpeg
jpeg 	image/jpeg
jpg 	image/jpeg
js 	application/x-javascript
kar 	audio/midi
latex 	application/x-latex
lha 	application/octet-stream
lzh 	application/octet-stream
m3u 	audio/x-mpegurl
m4a 	audio/mp4a-latm
m4b 	audio/mp4a-latm
m4p 	audio/mp4a-latm
m4u 	video/vnd.mpegurl
m4v 	video/x-m4v
mac 	image/x-macpaint
man 	application/x-troff-man
mathml 	application/mathml+xml
me 	application/x-troff-me
mesh 	model/mesh
mid 	audio/midi
midi 	audio/midi
mif 	application/vnd.mif
mov 	video/quicktime
movie 	video/x-sgi-movie
mp2 	audio/mpeg
mp3 	audio/mpeg
mp4 	video/mp4
mpe 	video/mpeg
mpeg 	video/mpeg
mpg 	video/mpeg
mpga 	audio/mpeg
ms 	application/x-troff-ms
msh 	model/mesh
mxu 	video/vnd.mpegurl
nc 	application/x-netcdf
oda 	application/oda
ogg 	application/ogg
pbm 	image/x-portable-bitmap
pct 	image/pict
pdb 	chemical/x-pdb
pdf 	application/pdf
pgm 	image/x-portable-graymap
pgn 	application/x-chess-pgn
pic 	image/pict
pict 	image/pict
png 	image/png
pnm 	image/x-portable-anymap
pnt 	image/x-macpaint
pntg 	image/x-macpaint
ppm 	image/x-portable-pixmap
ppt 	application/vnd.ms-powerpoint
ps 	application/postscript
qt 	video/quicktime
qti 	image/x-quicktime
qtif 	image/x-quicktime
ra 	audio/x-pn-realaudio
ram 	audio/x-pn-realaudio
ras 	image/x-cmu-raster
rdf 	application/rdf+xml
rgb 	image/x-rgb
rm 	application/vnd.rn-realmedia
roff 	application/x-troff
rtf 	text/rtf
rtx 	text/richtext
sgm 	text/sgml
sgml 	text/sgml
sh 	application/x-sh
shar 	application/x-shar
silo 	model/mesh
sit 	application/x-stuffit
skd 	application/x-koan
skm 	application/x-koan
skp 	application/x-koan
skt 	application/x-koan
smi 	application/smil
smil 	application/smil
snd 	audio/basic
so 	application/octet-stream
spl 	application/x-futuresplash
src 	application/x-wais-source
sv4cpio 	application/x-sv4cpio
sv4crc 	application/x-sv4crc
svg 	image/svg+xml
swf 	application/x-shockwave-flash
t 	application/x-troff
tar 	application/x-tar
tcl 	application/x-tcl
tex 	application/x-tex
texi 	application/x-texinfo
texinfo 	application/x-texinfo
tif 	image/tiff
tiff 	image/tiff
tr 	application/x-troff
tsv 	text/tab-separated-values
txt 	text/plain
ustar 	application/x-ustar
vcd 	application/x-cdlink
vrml 	model/vrml
vxml 	application/voicexml+xml
wav 	audio/x-wav
wbmp 	image/vnd.wap.wbmp
wbmxl 	application/vnd.wap.wbxml
wml 	text/vnd.wap.wml
wmlc 	application/vnd.wap.wmlc
wmls 	text/vnd.wap.wmlscript
wmlsc 	application/vnd.wap.wmlscriptc
wrl 	model/vrml
xbm 	image/x-xbitmap
xht 	application/xhtml+xml
xhtml 	application/xhtml+xml
xls 	application/vnd.ms-excel
xml 	application/xml
xpm 	image/x-xpixmap
xsl 	application/xml
xslt 	application/xslt+xml
xul 	application/vnd.mozilla.xul+xml
xwd 	image/x-xwindowdump
xyz 	chemical/x-xyz
zip 	application/zip 


 
 */
