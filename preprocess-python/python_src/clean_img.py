import matplotlib.pyplot as plt
import cv2
import os

def rectify(y):
    x = y.copy()
    if (x[0] == 0 and x[1] == 0 and x[2] == 0):
        return x
    elif (x[0] == 255 and x[1] == 0 and x[2] == 0):
        x[0] = 0
        return x
    else:
        x[0] = 255
        x[1] = 255
        x[2] = 255
        return x

def rect_img(img):
    for i in range(img.shape[0]):
        for j in range(img.shape[1]):
            temp = rectify(img[i,j])
            img[i,j,0] = temp[0]
            img[i,j,1] = temp[1]
            img[i,j,2] = temp[2]
    return img

corners = [(348,101), (160,427), (536,427)]
border = [(90, 150), (440, 550)]

def clean_img(img, base):
    img_clean = rect_img(img)
    base_clean = rect_img(base)

    diff = base_clean-img_clean

    cv2.line(diff, corners[0], corners[1], (255, 255, 255), thickness=1)
    cv2.line(diff, corners[1], corners[2], (255, 255, 255), thickness=1)
    cv2.line(diff, corners[2], corners[0], (255, 255, 255), thickness=1)

    crop = diff[border[0][0]:border[1][0], border[0][1]: border[1][1],:]
    
    return crop

img_fldr = '/Users/shivampandey/projects/isotherm-viz/raw_data/'
base_file = '/Users/shivampandey/projects/isotherm-viz/raw_data/320.bmp'
out_fldr = "/Users/shivampandey/projects/isotherm-viz/rectified_img/"

img_list = os.listdir(img_fldr)

base_r = plt.imread(base_file)
base = base_r.copy()

for f in img_list:
    img_file = img_fldr + f
    out_file = out_fldr + f
    out_file = out_file[:-3]
    out_file += 'png'

    img_r = plt.imread(img_file)
    img = img_r.copy()

    out = clean_img(img, base)

    cv2.imwrite(out_file, out)